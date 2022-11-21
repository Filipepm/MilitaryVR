//#define ENABLE_NATIVE_LOGGING
using System;
using System.Collections;
using System.IO;
#if (NET_4_6 || USE_ASYNC)
using System.Threading;
using System.Threading.Tasks;
#endif
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#if USE_EDITOR_COROUTINE
using Unity.EditorCoroutines.Editor;
#endif
#endif

#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

[RequireComponent(typeof(BoxCollider), typeof(MeshRenderer), typeof(MeshFilter))]
public class HoloVideoObject : MonoBehaviour, IDisposable
{
    // settings available in Unity editor
    [Tooltip("Use relative path from streaming assets or full path")]
    public string Url;
    public bool ShouldAutoPlay;
    [SerializeField] private float _audioVolume = 1.0f;
    public float AudioVolume
    {
        get
        {
            return _audioVolume;
        }
        set
        {
            _audioVolume = value;
            if (pluginInterop != null)
            {
                pluginInterop.SetAudioVolume(value);
            }
        }
    }

    public Vector3 audioSourceOffset;
    public bool flipHandedness = false;
    [SerializeField] public float _clockScale = 1.0f;
    public float ClockScale
    {
        get
        {
            if (pluginInterop != null)
            {
                return pluginInterop.GetClockScale();
            }
            return 0.0f;
        }
        set
        {
            _clockScale = value;
            if (pluginInterop != null)
            {
                pluginInterop.SetClockScale(value);
            }
        }
    }

    public bool computeNormals = true;

    public SVFOpenInfo Settings = new SVFOpenInfo()
    {
        AudioDisabled = false,
        AutoLooping = true,
        RenderViaClock = true,
        OutputNormals = true,
        StartDownloadOnOpen = false,
        PlaybackRate = 1.0f,
        AudioDeviceId = null,
        forceSoftwareClock = false,
        HRTFCutoffDistance = float.MaxValue,
        HRTFGainDistance = 1.0f,
        HRTFMinGain = -10.0f,
        HRTFMaxGain = 12.0f
    };

    [Tooltip("We usually read max number of vertices from SVF file, as part of SVFFileInfo."
     + " However, if we fail to load it from file, we default to this value")]
    public uint DefaultMaxVertexCount = 15000;
    public uint DefaultMaxIndexCount = 45000; // see comment above

    private Bounds localSpaceBounds = new Bounds();
    public Bounds BoundingBox
    {
        get { return HVCollider.bounds; }
    }

    // Bounding box mesh (with no material) required so that this will survive culling correctly
    private Mesh[] meshes = { null, null }; // PS4 requires explicit double buffer
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private BoxCollider HVCollider;
    private Material material;

    private AudioListener listener = null;

    protected SVFUnityPluginInterop pluginInterop = null;
    protected bool isInitialized = false;
    protected SVFOpenInfo openInfo;
    public SVFFileInfo fileInfo = new SVFFileInfo();
    protected SVFFrameInfo lastFrameInfo = new SVFFrameInfo();

    public delegate void OnOpenEvent(HoloVideoObject sender, string url);
    public delegate void OnFrameInfoEvent(HoloVideoObject sender, SVFFrameInfo frameInfo);
    public delegate void OnRenderEvent(HoloVideoObject sender);
    public delegate void OnEndOfStream(HoloVideoObject sender);
    public delegate void OnFatalErrorEvent(HoloVideoObject sender);

    public OnOpenEvent OnOpenNotify = null;
    public OnFrameInfoEvent OnUpdateFrameInfoNotify = null;
    public OnRenderEvent OnRenderCallback = null;
    public OnFatalErrorEvent OnFatalError = null;
    public OnEndOfStream OnEndOfStreamNotify = null; // derived class should register for this event

#if UNITY_EDITOR && USE_EDITOR_COROUTINE
    private EditorCoroutine UnityBufferCoroutine;
#else
    private Coroutine UnityBufferCoroutine;
#endif
#if NET_4_6
    private static Thread mainThread;
#endif

    private Logger logger = new Logger(Debug.unityLogger.logHandler);
    private const string TAG = "HoloVideoObject";
    public bool IsPlaying
    {
        get { return isPlaying; }
    }
    private bool isPlaying = false;
    private bool ShouldPauseAfterPlay = false;
    private bool shouldPlayWhenReady = false;
    [Tooltip("Usually okay to leave at 1, but try 2 if your first frame's drawing a blank")]
    public uint PauseFrameID = 1;

#if UNITY_ANDROID && !UNITY_EDITOR
    private static AndroidJavaObject javaPlugin;
    private static readonly object javaPluginLock = new object();
#endif

    void Awake()
    {
#if NET_4_6
        mainThread = Thread.CurrentThread;
#endif
        HVCollider = GetComponent<BoxCollider>();
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.material;

#if UNITY_ANDROID && !UNITY_EDITOR
        lock (javaPluginLock)
        {
            if (javaPlugin == null)
            {
                var obbPath = Application.dataPath.Contains(".obb") ? Application.dataPath : null;
                var unityActivityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject unityActivity = unityActivityClass.GetStatic<AndroidJavaObject>("currentActivity");
                javaPlugin = new AndroidJavaObject("com.SVFUnityPlugin.JavaPlugin", new object[] { unityActivity, obbPath });
            }
        }
#endif
    }

    void Start()
    {
        if (ShouldAutoPlay)
        {
            Open(Url);
            Play();
        }
    }

    private IEnumerator FillUnityBuffers()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            if (isInitialized && isPlaying)
            {
                pluginInterop.IssueUnityRenderModePluginEvent();
            }
        }
    }

    void OnRenderObject()
    {
        if (isInitialized)
        {
            HandleOnRender();
        }
    }

    public void URLChange(string url)
    {
        Url = url;
    }

    public void OverridePlay()
    {
            Open(Url);
            Play();
    }

    public void AudioToggle()
    {
        listener = null;
    }

    public void OnEnable()
    {
        OnUpdateFrameInfoNotify += PauseOnFrame;
#if UNITY_EDITOR
        EditorApplication.pauseStateChanged += OnPauseStateChange;
#endif
    }

    public void OnDisable()
    {
        OnUpdateFrameInfoNotify -= PauseOnFrame;
#if UNITY_EDITOR
        EditorApplication.pauseStateChanged -= OnPauseStateChange;
#endif

    }

    // 3D Audio Support
    private AudioListener Listener()
    {
        if (listener == null)
        {
            AudioListener[] listeners = FindObjectsOfType<AudioListener>();
            foreach (var li in listeners)
            {
                if (li.isActiveAndEnabled)
                {
                    listener = li;
                    break;
                }
            }
        }
        return listener;
    }

    protected void Update3DAudio()
    {
        if (pluginInterop != null && pluginInterop.IsPlaying())
        {
            var li = Listener();
            if (li != null)
            {
                Vector3 pos = li.transform.InverseTransformPoint(transform.position + audioSourceOffset);
                pluginInterop.SetHCapObjectAudio3DPosition(pos.x, pos.y, pos.z);
            }
        }
    }

    public void Update()
    {
        if (!isInitialized || pluginInterop == null || !isPlaying)
        {
            return;
        }

        Update3DAudio();

        // pass a temp frame info struct here so we don't trash information in lastFrameInfo when GetHCapObjectFrameInfo returns a bunch of zeros.
        SVFFrameInfo frameInfo = new SVFFrameInfo();
        if (pluginInterop.GetHCapObjectFrameInfo(ref frameInfo))
        {
            // if we didn't get any useful frame info, don't overwrite our previous values (likely initial file-level values)
            if (frameInfo.textureWidth > 0 && frameInfo.textureHeight > 0)
            {
                lastFrameInfo = frameInfo;

                SVFFrameInfo adjustedFrameInfo = lastFrameInfo;

                if ((adjustedFrameInfo.maxX - adjustedFrameInfo.minX) > 0.0f ||
                    (adjustedFrameInfo.maxY - adjustedFrameInfo.minY) > 0.0f ||
                    (adjustedFrameInfo.maxZ - adjustedFrameInfo.minZ) > 0.0f)
                {
                    Vector3 min = new Vector3((float)adjustedFrameInfo.minX, (float)adjustedFrameInfo.minY, (float)adjustedFrameInfo.minZ);
                    Vector3 max = new Vector3((float)adjustedFrameInfo.maxX, (float)adjustedFrameInfo.maxY, (float)adjustedFrameInfo.maxZ);
                    localSpaceBounds.SetMinMax(min, max);

                    HVCollider.center = localSpaceBounds.center;
                    HVCollider.size = localSpaceBounds.size;
                    meshFilter.mesh.bounds = localSpaceBounds;
                }

#if UNITY_PS4 && !UNITY_EDITOR
                UpdateUnityBuffers(false, true);
#else
                UpdateUnityBuffers();
#endif
                HandleOnUpdateFrameInfo(adjustedFrameInfo);

                if (lastFrameInfo.isEOS)
                {
                    if (null != OnEndOfStreamNotify)
                    {
                        OnEndOfStreamNotify(this);
                    }
                }
            }
        }
        else
        {
            logger.Log("GetHCapObjectFrameInfo returned false");
        }
#if ENABLE_NATIVE_LOGGING
        string[] trace = pluginInterop.GetTrace();
        if(null != trace)
        {
            foreach(var line in trace)
            {
                logger.Log(line);
            }
        }
#endif
    }

    private static uint Roundup(uint x, uint multiple)
    {
        if (multiple == 0)
        {
            return x;
        }

        uint remainder = x % multiple;

        if (remainder == 0)
        {
            return x;
        }

        return x + multiple - remainder;
    }

    protected void CreateTexture(int width, int height)
    {
        if (meshRenderer.material.mainTexture)
        {
            Destroy(meshRenderer.material.mainTexture);
            meshRenderer.material.mainTexture = null;
        }

        bool mipmaps = false;

        var format = TextureFormat.BGRA32;
        if (Application.platform == RuntimePlatform.Android || 
            SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLCore)
        {
            format = TextureFormat.RGBA32;
        }

#if PLATFORM_LUMIN && !UNITY_EDITOR
        format = TextureFormat.RGBA32;
#endif
        var tex = new Texture2D(width, height, format, mipmaps);
        tex.Apply();

        meshRenderer.material.mainTexture = tex;
        meshRenderer.material.SetTexture("_EmissionMap", meshRenderer.material.mainTexture);
        // For lightweight RP:
        meshRenderer.material.SetTexture("_BaseMap", meshRenderer.material.mainTexture);
        //For other shaders, attach texture as map here
    }

    protected void UpdateUnityBuffers(bool forceNewMesh = false, bool meshDoubleBuffer = false)
    {
        bool needsBufferUpdate = false;

        Texture2D tex = (Texture2D) meshRenderer.material.mainTexture;
        if (!tex || tex.width != lastFrameInfo.textureWidth || tex.height != lastFrameInfo.textureHeight)
        {
            CreateTexture(lastFrameInfo.textureWidth, lastFrameInfo.textureHeight);
            tex = (Texture2D)meshRenderer.material.mainTexture;
            needsBufferUpdate = true;
        }

        if (meshDoubleBuffer)
        {
            if (meshes[0] == null)
            { // Initialize the double Mesh array
                meshes[0] = meshFilter.mesh;
                meshes[1] = new Mesh();
            }
            else
            { // Flip active Mesh
                if (meshFilter.mesh == meshes[0])
                {
                    meshFilter.mesh = meshes[1];
                }
                else
                {
                    meshFilter.mesh = meshes[0];
                }
            }
            needsBufferUpdate = true;
        }

        Mesh mesh = meshFilter.mesh;
        if (forceNewMesh || mesh.vertexCount < lastFrameInfo.vertexCount ||
            mesh.GetIndexCount(0) < lastFrameInfo.indexCount)
        {
            //Debug.Log("New vertex/index buffers for " + name + " " + lastFrameInfo.vertexCount + " " + lastFrameInfo.indexCount);

            if (pluginInterop != null)
            {
                pluginInterop.ReleaseUnityBuffers();
            }

            // Round up to some reasonable buffer size so that we're not regenerating this for every frame
            uint newVertexCount = Roundup(lastFrameInfo.vertexCount, 5000);

            mesh.Clear();
            mesh.MarkDynamic();
            mesh.indexFormat = IndexFormat.UInt32;
            mesh.vertices = new Vector3[newVertexCount];
            mesh.normals = new Vector3[newVertexCount];
            mesh.uv = new Vector2[newVertexCount];

            // ...and assure that indices are a multiple of 3
            uint newIndexCount = Roundup(Roundup(lastFrameInfo.indexCount, 5000), 6);
            int[] indices = new int[newIndexCount];
#if UNITY_PS4 && !UNITY_EDITOR
            // On PS4, Unity will optimize the vertex buffer size by the maximum index
            indices[0] = (int)newVertexCount - 1;
#endif
            mesh.triangles = indices;
            needsBufferUpdate = true;
        }

        if (pluginInterop != null)
        {
            if (!pluginInterop.TestUnityBuffersValid())
            {
                Debug.LogWarning("TestUnityBuffersValid -> false");
                needsBufferUpdate = true;
            }

            if (mesh.vertexBufferCount > 0 && needsBufferUpdate)
            {
                //Debug.Log("UUB-set " + mesh.vertexCount + " " + mesh.GetIndexCount(0));
                pluginInterop.SetUnityBuffers(tex.GetNativeTexturePtr(), tex.width, tex.height,
                   mesh.GetNativeVertexBufferPtr(0), mesh.vertexCount,
                   mesh.GetNativeIndexBufferPtr(), (int) mesh.GetIndexCount(0));
            }
        }
    }

    virtual protected void HandleOnUpdateFrameInfo(SVFFrameInfo frameInfo)
    {
        if (null != OnUpdateFrameInfoNotify)
        {
            OnUpdateFrameInfoNotify(this, frameInfo);
        }
    }

    virtual protected void HandleOnOpen(string url)
    {
        if (null != OnOpenNotify)
        {
            OnOpenNotify(this, url);
        }
    }

    virtual protected void HandleOnRender()
    {
        if (null != OnRenderCallback)
        {
            OnRenderCallback(this);
        }
    }

    public bool Open(string urlPath)
    {
        if (Open(urlPath, transform.localScale))
        {
            PostOpenMainThreadSetup(urlPath);
            return true;
        }
        return false;
    }

    private static bool CheckURLValid(string source)
    {
        Uri uriResult;
        return Uri.TryCreate(source, UriKind.Absolute, out uriResult) &&
        (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }

    // Common entry point for both OpenAsync and Open to have transform data only accessible on main thread
    public bool Open(string urlPath, Vector3 defaultBoundsScale)
    {
        if (!isInitialized)
        {
            if (!Initialize())
            {
                return false;
            }
        }

        urlPath = ConstructFullUrl(urlPath);

        bool res = pluginInterop.OpenHCapObject(urlPath, ref Settings);
        isPlaying = false;
        if (res)
        {
            ClockScale = _clockScale;
            AudioVolume = _audioVolume;
            Url = urlPath;

            if (pluginInterop.GetHCapObjectFileInfo(ref fileInfo))
            {
                Vector3 min = Vector3.zero;
                Vector3 max = Vector3.zero;
                if ((fileInfo.maxX - fileInfo.minX) > 0.0f ||
                    (fileInfo.maxY - fileInfo.minY) > 0.0f ||
                    (fileInfo.maxZ - fileInfo.minZ) > 0.0f)
                {
                    min = new Vector3((float) fileInfo.minX, (float) fileInfo.minY, (float) fileInfo.minZ);
                    max = new Vector3((float) fileInfo.maxX, (float) fileInfo.maxY, (float) fileInfo.maxZ);
                }
                else // We need some sane bounds even if the hcap file doesn't tell us.
                {
                    min = new Vector3(0.5f / defaultBoundsScale.x, 0.5f / defaultBoundsScale.y, 0.5f / defaultBoundsScale.z);
                    max = new Vector3(1.1f / defaultBoundsScale.x, 1.1f / defaultBoundsScale.y, 1.1f / defaultBoundsScale.z);
                }
                localSpaceBounds.SetMinMax(min, max);
            }
            // Initialize with reasonable numbers if available.  Important for index and vertex buffers to avoid reallocation in the middle of playback
            lastFrameInfo.indexCount = fileInfo.maxIndexCount > DefaultMaxIndexCount ? fileInfo.maxIndexCount : DefaultMaxIndexCount;
            lastFrameInfo.vertexCount = fileInfo.maxVertexCount > DefaultMaxVertexCount ? fileInfo.maxVertexCount : DefaultMaxVertexCount;
            lastFrameInfo.textureWidth = (int) fileInfo.fileWidth;
            lastFrameInfo.textureHeight = (int) fileInfo.fileHeight;
        }
        else
        {
            logger.LogError(TAG, "HoloVideoObject::Open Error: unable to OpenHCapObject()");
        }
        return res;
    }

#if (NET_4_6 || USE_ASYNC)
    public async void OpenAsync(bool shouldPlayWhenReady = false)
    {
        this.shouldPlayWhenReady = shouldPlayWhenReady;
        Initialize();
        Url = ConstructFullUrl(Url);
        Vector3 defaultBoundsScale = transform.localScale;
        bool wasOpened = await Task.Factory.StartNew(() =>
        {
            return Open(Url, defaultBoundsScale);
        });
        if (wasOpened)
        {
            PostOpenMainThreadSetup(Url);
            if (this.shouldPlayWhenReady)
            {
                Play();
            }
        }
    }
#endif

    private string ConstructFullUrl(string urlPath)
    {
        // On Android a bare filename means a file in StreamingAssets which can be loaded via Android asset manager API,
        // and an absolute path will be opened directly, so just pass unmodified 'urlPath' directly through to plugin.
        if (Application.platform == RuntimePlatform.Android)
        {

        }
        else if (Path.GetExtension(urlPath) == ".mpd" || Path.GetExtension(urlPath) == ".m3u8")
        {
            // is it a file path to an m3u8 file or a real URL
            if (!CheckURLValid(urlPath) && !Path.IsPathRooted(urlPath))
            {
                urlPath = Application.streamingAssetsPath + "/" + urlPath;
            }
        }
        else if (!Path.IsPathRooted(urlPath))
        {
            urlPath = Application.streamingAssetsPath + "/" + urlPath;
        }
        return urlPath;
    }

    // After opening, set up transform, coroutine, etc. that can only run on main thread
    // Should be called by both OpenAsync and Open
    void PostOpenMainThreadSetup(string urlPath)
    {
#if NET_4_6
        Debug.Assert(Thread.CurrentThread == mainThread);
#endif
        HVCollider.center = localSpaceBounds.center;
        HVCollider.size = localSpaceBounds.size;
        meshFilter.mesh.bounds = localSpaceBounds;

        Update3DAudio();
        UpdateUnityBuffers(true);

        if (UnityBufferCoroutine != null)
        {
#if UNITY_EDITOR && USE_EDITOR_COROUTINE
            EditorCoroutineUtility.StopCoroutine(UnityBufferCoroutine);
#else
            StopCoroutine(UnityBufferCoroutine);
#endif
        }
#if UNITY_EDITOR && USE_EDITOR_COROUTINE
        UnityBufferCoroutine = EditorCoroutineUtility.StartCoroutineOwnerless(FillUnityBuffers());
#else
        UnityBufferCoroutine = StartCoroutine(FillUnityBuffers());
#endif

        HandleOnOpen(urlPath);
    }

    public bool Play()
    {
        if (pluginInterop == null)
        {
            return false;
        }
        meshRenderer.enabled = true;
        isPlaying = true;
        return pluginInterop.PlayHCapObject();
    }

    public bool Pause()
    {
        if (pluginInterop == null)
        {
            return false;
        }
        shouldPlayWhenReady = false;
        isPlaying = false;
        return pluginInterop.PauseHCapObject();
    }

    public void DisplayFrame(uint frameToStopOn = 1)
    {
        ShouldPauseAfterPlay = true;
        PauseFrameID = frameToStopOn;
        meshRenderer.enabled = true;
        Rewind();
        Play();
    }

    private void PauseOnFrame(HoloVideoObject sender, SVFFrameInfo frameInfo)
    {
        if (ShouldPauseAfterPlay && frameInfo.frameId >= PauseFrameID)
        {
            Pause();
            ShouldPauseAfterPlay = false;
        }
    }

    public bool Rewind()
    {
        if (pluginInterop == null)
        {
            return false;
        }
        return pluginInterop.RewindHCapObject();
    }

    /// <summary>
    /// Pauses and rewinds the HVO and disables the mesh renderer to keep it hidden.
    /// Keeps the HVO loaded in memory.
    /// </summary>
    /// <returns> Whether both Pause and Rewind was successful </returns>
    public bool Stop()
    {
        if (pluginInterop == null)
        {
            return false;
        }
        meshRenderer.enabled = false;
        return Pause() && Rewind();
    }

    public bool Close()
    {
        shouldPlayWhenReady = false;

        if (meshRenderer)
        {
            meshRenderer.enabled = false;

            Texture2D tex = (Texture2D)meshRenderer.material.mainTexture;
            if (tex != null)
            {
#if UNITY_EDITOR // The Preview needs to hang on to the texture even after Close()
                if (GetComponent<HoloVideoPreview>() == null)
#endif
                {
                    Destroy(tex);
                    meshRenderer.material.mainTexture = null;
                    isPlaying = false;
                }
            }
        }

        if (pluginInterop == null)
        {
            return false;
        }

        return pluginInterop.CloseHCapObject();
    }

#if (NET_4_6 || USE_ASYNC)
    public async void CloseAsync()
    {
        meshRenderer.enabled = false;
        await Task.Factory.StartNew(() =>
        {
            pluginInterop.CloseHCapObject();
        });
    }
#endif

    public void Cleanup()
    {
        isInitialized = false;

        if (pluginInterop != null)
        {
            pluginInterop.Dispose();
            pluginInterop = null;

            Url = "";
            fileInfo = new SVFFileInfo();
            openInfo = new SVFOpenInfo();
            lastFrameInfo = new SVFFrameInfo();
        }
    }

    public SVFPlaybackState GetCurrentState()
    {
        if (pluginInterop == null)
        {
            return SVFPlaybackState.Empty;
        }
        return pluginInterop.GetHCapState();
    }

    private bool Initialize()
    {
        try
        {
            if (null != pluginInterop)
            {
                pluginInterop.Dispose();
            }

            pluginInterop = new SVFUnityPluginInterop();

#if UNITY_PS4 && !UNITY_EDITOR
            HCapPs4GlobalParamsInterop globalParams = new HCapPs4GlobalParamsInterop()
            {
                PlayerWorkMemorySize = 1024 * 1024 * 1024,
                PlayerVideoMemorySize = 512 * 1024 * 1024,
                AudioGranularity = 512,
                Audio3dAffinity = 0x40, //0b01000000,
                AmbisonicLevel = 3,
                AmbisonicCutoffRadius = 10f,
            };
            pluginInterop.InitializeHCapPlugin(globalParams);
#endif

            HCapSettingsInterop hcapSettings = new HCapSettingsInterop()
            {
                defaultMaxVertexCount = DefaultMaxVertexCount,
                defaultMaxIndexCount = DefaultMaxIndexCount,

#if UNITY_PS4
                StreamMode = 1,
                StreamCacheSize = 512 * 1024,
                StreamCacheNum = 12,
                DecodePipelineSize = 6,
                DemuxVideoBufferSize = 16 * 1024 * 1024,
                MeshDecoderAffinity = 0x30, //0b00110000
                AudioDecoderAffinity = 0x3f, //0b00111111
                VideoDecoderAffinity = 0x3f, //0b00111111
                DemuxerAffinity = 0x3f, //0b00111111
                ControllerAffinity = 0x3f, //0b00111111
                StreamingAffinity = 0x3f, //0b00111111
#endif
            };

#if ENABLE_NATIVE_LOGGING
            pluginInterop.EnableTracing(true, LogLevel.Message);
#endif

            pluginInterop.CreateHCapObject(hcapSettings);

            if (meshRenderer == null)
            {
                meshRenderer = GetComponent<MeshRenderer>();
            }
            if (meshFilter == null)
            {
                meshFilter = GetComponent<MeshFilter>();
            }
            if (HVCollider == null)
            {
                HVCollider = GetComponent<BoxCollider>();
            }

            meshRenderer.material = material;
            meshFilter.mesh.bounds = localSpaceBounds;
            pluginInterop.SetComputeNormals(computeNormals);

            isInitialized = true;
        }
        catch (Exception ex)
        {
            logger.LogException(ex);
            isInitialized = false;
            return false;
        }
        return true;
    }

    private bool wasPlaying = false;

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            wasPlaying = isPlaying;
            Pause();
        }
        else
        {
            if (wasPlaying)
            {
                Play();
            }
        }
    }

    private void OnApplicationQuit()
    {
        OnDestroy();
    }

    private void OnDestroy()
    {
        Close();
        Cleanup();
    }

    private bool isInstanceDisposed = false;
    protected virtual void Dispose(bool disposing)
    {
        if (!isInstanceDisposed)
        {
            isInstanceDisposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~HoloVideoObject()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(false);
    }

    public AudioDeviceInfo[] GetAudioDevices()
    {
        return SVFUnityPluginInterop.EnumerateAudioDevices();
    }

    public string GetVideoFolder()
    {
#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_XBOXONE || UNITY_WSA)
        return SVFUnityPluginInterop.GetVideoFolder();
#else
        return null;
#endif
    }

#if UNITY_EDITOR
    void OnPauseStateChange(PauseState state)
    {
        OnApplicationPause(state == PauseState.Paused);
    }
#endif
}