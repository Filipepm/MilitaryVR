using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Rendering;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct HCapSettingsInterop
{
    [MarshalAs(UnmanagedType.U4)]
    public uint defaultMaxVertexCount;
    [MarshalAs(UnmanagedType.U4)]
    public uint defaultMaxIndexCount;

#if UNITY_PS4
    [MarshalAs(UnmanagedType.U4)]
    public uint StreamMode;
    [MarshalAs(UnmanagedType.U4)]
    public uint StreamCacheSize;
    [MarshalAs(UnmanagedType.U4)]
    public uint StreamCacheNum;
    [MarshalAs(UnmanagedType.U4)]
    public uint DecodePipelineSize;
    [MarshalAs(UnmanagedType.U4)]
    public uint DemuxVideoBufferSize;
    [MarshalAs(UnmanagedType.U4)]
    public uint MeshDecoderAffinity;
    [MarshalAs(UnmanagedType.U4)]
    public uint AudioDecoderAffinity;
    [MarshalAs(UnmanagedType.U4)]
    public uint VideoDecoderAffinity;
    [MarshalAs(UnmanagedType.U4)]
    public uint DemuxerAffinity;
    [MarshalAs(UnmanagedType.U4)]
    public uint ControllerAffinity;
    [MarshalAs(UnmanagedType.U4)]
    public uint StreamingAffinity;
#endif
};

#if UNITY_PS4 && !UNITY_EDITOR
[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct HCapPs4GlobalParamsInterop
{
    [MarshalAs(UnmanagedType.U4)]
    public uint PlayerWorkMemorySize;
    [MarshalAs(UnmanagedType.U4)]
    public uint PlayerVideoMemorySize;
    [MarshalAs(UnmanagedType.U4)]
    public uint AudioGranularity;
    [MarshalAs(UnmanagedType.U4)]
    public uint Audio3dAffinity;
    [MarshalAs(UnmanagedType.U4)]
    public uint AmbisonicLevel;
    [MarshalAs(UnmanagedType.R4)]
    public float AmbisonicCutoffRadius;
};
#endif

[Serializable]
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct SVFOpenInfo
{
    [MarshalAs(UnmanagedType.I1)]
    public bool AudioDisabled;
    [MarshalAs(UnmanagedType.I1)]
    public bool UseKeyedMutex;
    [MarshalAs(UnmanagedType.I1)]
    public bool RenderViaClock;
    [MarshalAs(UnmanagedType.I1)]
    public bool OutputNormals;
    [MarshalAs(UnmanagedType.I1)]
    public bool StartDownloadOnOpen;
    [MarshalAs(UnmanagedType.I1)]
    public bool AutoLooping;
    [MarshalAs(UnmanagedType.I1)]
    public bool forceSoftwareClock;
    [MarshalAs(UnmanagedType.R4)]
    public float PlaybackRate;
    [MarshalAs(UnmanagedType.R4)]
    public float HRTFMinGain;
    [MarshalAs(UnmanagedType.R4)]
    public float HRTFMaxGain;
    [MarshalAs(UnmanagedType.R4)]
    public float HRTFGainDistance;
    [MarshalAs(UnmanagedType.R4)]
    public float HRTFCutoffDistance;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
    public string AudioDeviceId;
}

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct SVFFileInfo
{
    [MarshalAs(UnmanagedType.I1)]
    public bool hasAudio;
    [MarshalAs(UnmanagedType.U8)]
    public ulong duration100ns;
    [MarshalAs(UnmanagedType.U4)]
    public uint frameCount;
    [MarshalAs(UnmanagedType.U4)]
    public uint maxVertexCount;
    [MarshalAs(UnmanagedType.U4)]
    public uint maxIndexCount;
    [MarshalAs(UnmanagedType.R4)]
    public float bitrateMbps;
    [MarshalAs(UnmanagedType.R4)]
    public float fileSize;
    [MarshalAs(UnmanagedType.R8)]
    public double minX;
    [MarshalAs(UnmanagedType.R8)]
    public double minY;
    [MarshalAs(UnmanagedType.R8)]
    public double minZ;
    [MarshalAs(UnmanagedType.R8)]
    public double maxX;
    [MarshalAs(UnmanagedType.R8)]
    public double maxY;
    [MarshalAs(UnmanagedType.R8)]
    public double maxZ;
    [MarshalAs(UnmanagedType.U4)]
    public uint fileWidth;
    [MarshalAs(UnmanagedType.U4)]
    public uint fileHeight;
    [MarshalAs(UnmanagedType.I1)]
    public bool hasNormals;
}

public enum SVFReaderStateInterop
{
    Unknown = 0,        //!< Reader is in unknown state
    Initialized = 1,    //!< Reader was properly initialized
    OpenPending = 2,    //!< Open is pending
    Opened = 3,         //!< Open finished
    Prerolling = 4,     //!< File opened and pre-roll started
    Ready = 5,          //!< Frames ready to be delivered
    Buffering = 6,      //!< Reader is buffering frames (i.e. frames not available for delivery)
    Closing = 7,        //!< File being closed
    Closed = 8,         //!< Filed closed
    EndOfStream = 9,    //!< Reached end of current file
    ShuttingDown = 10,   //!< SVFReader is in process of shutting down
};

public enum VRChannel
{
    Mono = 0,
    Left = 1,
    Right = 2,
    Center = 3,
    Head = 4
};

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct SVFStatusInterop
{
    [MarshalAs(UnmanagedType.I1)]
    public bool isLiveSVFSource;
    [MarshalAs(UnmanagedType.U4)]
    public UInt32 lastReadFrame;
    [MarshalAs(UnmanagedType.U4)]
    public UInt32 unsuccessfulReadFrameCount;
    [MarshalAs(UnmanagedType.U4)]
    public UInt32 droppedFrameCount;
    [MarshalAs(UnmanagedType.U4)]
    public UInt32 errorHresult;
    [MarshalAs(UnmanagedType.I4)]
    public int lastKnownState; // cast of SVFReaderStateInterop
}

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct SVFFrameInfo
{
    [MarshalAs(UnmanagedType.U8)]
    public ulong frameTimestamp; // in 100ns MF time units
    [MarshalAs(UnmanagedType.R8)]
    public double minX;
    [MarshalAs(UnmanagedType.R8)]
    public double minY;
    [MarshalAs(UnmanagedType.R8)]
    public double minZ;
    [MarshalAs(UnmanagedType.R8)]
    public double maxX;
    [MarshalAs(UnmanagedType.R8)]
    public double maxY;
    [MarshalAs(UnmanagedType.R8)]
    public double maxZ;
    [MarshalAs(UnmanagedType.U4)]
    public uint frameId; // starts from 0
    [MarshalAs(UnmanagedType.U4)]
    public uint vertexCount; // per-frame vertex count
    [MarshalAs(UnmanagedType.U4)]
    public uint indexCount; // per-frame index count
    [MarshalAs(UnmanagedType.I4)]
    public int textureWidth;
    [MarshalAs(UnmanagedType.I4)]
    public int textureHeight;
    [MarshalAs(UnmanagedType.I1)]
    public bool isEOS;
    [MarshalAs(UnmanagedType.I1)]
    public bool isRepeatedFrame;
    [MarshalAs(UnmanagedType.I1)]
    public bool isKeyFrame;
};

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct Matrix4x4PluginInterop
{
    [MarshalAs(UnmanagedType.R4)]
    public float m00;
    [MarshalAs(UnmanagedType.R4)]
    public float m01;
    [MarshalAs(UnmanagedType.R4)]
    public float m02;
    [MarshalAs(UnmanagedType.R4)]
    public float m03;
    [MarshalAs(UnmanagedType.R4)]
    public float m10;
    [MarshalAs(UnmanagedType.R4)]
    public float m11;
    [MarshalAs(UnmanagedType.R4)]
    public float m12;
    [MarshalAs(UnmanagedType.R4)]
    public float m13;
    [MarshalAs(UnmanagedType.R4)]
    public float m20;
    [MarshalAs(UnmanagedType.R4)]
    public float m21;
    [MarshalAs(UnmanagedType.R4)]
    public float m22;
    [MarshalAs(UnmanagedType.R4)]
    public float m23;
    [MarshalAs(UnmanagedType.R4)]
    public float m30;
    [MarshalAs(UnmanagedType.R4)]
    public float m31;
    [MarshalAs(UnmanagedType.R4)]
    public float m32;
    [MarshalAs(UnmanagedType.R4)]
    public float m33;
}

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct Vector3Interop
{
    [MarshalAs(UnmanagedType.R4)]
    public float x;
    [MarshalAs(UnmanagedType.R4)]
    public float y;
    [MarshalAs(UnmanagedType.R4)]
    public float z;
}

public enum SVFPlaybackState
{
    Empty = 0,
    Initialized = 1,
    Opened = 2,
    Playing = 3,
    Paused = 4,
    Closed = 5,
    Broken = 6
}

public enum LogLevel
{
    None,
    Critical,
    Message,
    Verbose,
    Debug
}

[Serializable]
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct AudioDeviceInfo
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
    public string Name;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
    public string Id;
}

class SVFPlaybackStateHelper
{
    public static SVFPlaybackState FromInt(int val)
    {
        return (SVFPlaybackState)val;
    }
}

class SVFReaderStateHelper
{
    public static SVFReaderStateInterop FromInt(int val)
    {
        return (SVFReaderStateInterop)val;
    }
}

public class SVFUnityPluginInterop : IDisposable
{
#if UNITY_IPHONE && !UNITY_EDITOR_OSX
    const string DllName = "__Internal";
#else
    const string DllName = "SVFUnityPlugin";
#endif

#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_XBOXONE || UNITY_WSA)
    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "GetVideoFolder")]
    public static extern IntPtr InteropGetVideoFolder();
    public static string GetVideoFolder()
    {
        IntPtr videoFolderPath = InteropGetVideoFolder();
        return Marshal.PtrToStringUni(videoFolderPath);
    }
#endif

    private static bool s_logstack = false;
    private Logger logger = new Logger(Debug.unityLogger.logHandler);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, EntryPoint = "TestUnityBuffersValid")]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool InteropTestUnityBuffersValid(int instanceId);
    public bool TestUnityBuffersValid()
    {
        if (instanceId != InvalidID)
            return InteropTestUnityBuffersValid(instanceId);

        return false;
    }

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, EntryPoint = "ReleaseUnityBuffers")]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool InteropReleaseUnityBuffers(int instanceId);
    public bool ReleaseUnityBuffers()
    {
        if (instanceId != InvalidID)
            return InteropReleaseUnityBuffers(instanceId);

        return false;
    }

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, EntryPoint = "SetUnityBuffers")]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool InteropSetUnityBuffers(int instanceId, System.IntPtr texture, int w, int h,
        IntPtr vertexBuffer, int vertexCount, IntPtr indexBuffer, int indexCount);
    public void SetUnityBuffers(System.IntPtr texture, int w, int h,
        IntPtr vertexBuffer, int vertexCount, IntPtr indexBuffer, int indexCount)
    {
        if (instanceId != InvalidID)
            InteropSetUnityBuffers(instanceId, texture, w, h, vertexBuffer, vertexCount, indexBuffer, indexCount);
    }

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetUnityRenderModeEventFunc")]
    private static extern IntPtr InteropGetUnityRenderModeEventFunc();
    public void IssueUnityRenderModePluginEvent()
    {
        if (s_logstack)
            logger.Log("[INTEROP]IssuePluginEvent");
        if (instanceId != InvalidID)
        {
            GL.IssuePluginEvent(InteropGetUnityRenderModeEventFunc(), instanceId);
        }
    }

#if UNITY_PS4 && !UNITY_EDITOR
    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, EntryPoint = "InitializeHCapPlugin")]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern void InteropInitializeHCapPlugin(ref HCapPs4GlobalParamsInterop globalParams);
    public void InitializeHCapPlugin(HCapPs4GlobalParamsInterop globalParams)
    {
        if (s_logstack)
        {
            logger.Log("[INTEROP]InitializeHCapPlugin");
        }
        InteropInitializeHCapPlugin(ref globalParams);
    }
#endif

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, EntryPoint = "CreateHCapObject")]
    [return: MarshalAs(UnmanagedType.I4)]
    private static extern int InteropCreateHCapObject(ref HCapSettingsInterop hcapSettings);
    public void CreateHCapObject(HCapSettingsInterop hcapSettings)
    {
        if (s_logstack)
            logger.Log("[INTEROP]CreateHCapObject");
        if (instanceId != InvalidID)
        {
            throw new Exception("HCapObject already created");
        }
        instanceId = InteropCreateHCapObject(ref hcapSettings);
    }

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, EntryPoint = "DestroyHCapObject")]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool InteropDestroyHCapObject(int instanceID);
    public bool DestroyHCapObject()
    {
        if (s_logstack)
            logger.Log("[INTEROP]DestroyHCapObject");
        if (instanceId != InvalidID)
        {
            bool res = InteropDestroyHCapObject(instanceId);
            if (res)
            {
                instanceId = InvalidID;
            }
            return res;
        }
        return true;
    }

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
    [DllImport(DllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "OpenHCapObject")]
#else
    [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall, EntryPoint = "OpenHCapObject")]
#endif
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool InteropOpenHCapObject(int instanceId, string filePath, ref SVFOpenInfo openInfo);
    public bool OpenHCapObject(string filePath, ref SVFOpenInfo openInfo)
    {
        if (s_logstack)
            logger.Log("[INTEROP]OpenHCapObject");

        if (!string.IsNullOrEmpty(openInfo.AudioDeviceId))
            Debug.Log(string.Format("Audio device: {0} requested", openInfo.AudioDeviceId));

        if (instanceId == InvalidID)
        {
            return false;
        }
        return InteropOpenHCapObject(instanceId, filePath, ref openInfo);
    }

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, EntryPoint = "CloseHCapObject")]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool InteropCloseHCapObject(int instanceId);
    public bool CloseHCapObject()
    {
        if (s_logstack)
            logger.Log("[INTEROP]CloseHCapObject");
        if (instanceId == InvalidID)
        {
            return false;
        }
        return InteropCloseHCapObject(instanceId);
    }

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, EntryPoint = "PlayHCapObject")]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool InteropPlayHCapObject(int instanceId);
    public bool PlayHCapObject()
    {
        if (s_logstack)
            logger.Log("[INTEROP]PlayHCapObject");
        if (instanceId == InvalidID)
        {
            return false;
        }
        return InteropPlayHCapObject(instanceId);
    }

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, EntryPoint = "PauseHCapObject")]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool InteropPauseHCapObject(int instanceId);
    public bool PauseHCapObject()
    {
        if (s_logstack)
            logger.Log("[INTEROP]PauseHCapObject");
        if (instanceId == InvalidID)
        {
            return false;
        }
        return InteropPauseHCapObject(instanceId);
    }

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, EntryPoint = "RewindHCapObject")]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool InteropRewindHCapObject(int instanceId);
    public bool RewindHCapObject()
    {
        if (s_logstack)
            logger.Log("[INTEROP]RewindHCapObject");
        if (instanceId == InvalidID)
        {
            return false;
        }
        return InteropRewindHCapObject(instanceId);
    }

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetHCapState")]
    [return: MarshalAs(UnmanagedType.I4)]
    private static extern int InteropGetHCapState(int instanceId);
    public SVFPlaybackState GetHCapState()
    {
        if (s_logstack)
            logger.Log("[INTEROP]GetHCapState");
        if (instanceId == InvalidID)
        {
            return SVFPlaybackState.Broken;
        }
        int stateCode = InteropGetHCapState(instanceId);
        return SVFPlaybackStateHelper.FromInt(stateCode);
    }

    public bool IsPlaying()
    {
        return GetHCapState() == SVFPlaybackState.Playing;
    }

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetHCapSVFStatus", CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool InteropGetHCapSVFStatus(int instanceId, ref SVFStatusInterop svfStatus);
    public bool GetHCapSVFStatus(ref SVFStatusInterop svfStatus, ref SVFReaderStateInterop svfInternalState)
    {
        if (instanceId == InvalidID)
        {
            return false;
        }
        if (true == InteropGetHCapSVFStatus(instanceId, ref svfStatus))
        {
            svfInternalState = SVFReaderStateHelper.FromInt(svfStatus.lastKnownState);
            return true;
        }
        return false;
    }

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetHCapObjectFileInfo")]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool InteropGetHCapObjectFileInfo(int instanceId, ref SVFFileInfo info);
    public bool GetHCapObjectFileInfo(ref SVFFileInfo fileInfo)
    {
        if (s_logstack)
            logger.Log("[INTEROP]GetHCapObjectFileInfo");
        if (instanceId == InvalidID)
        {
            return false;
        }
        return InteropGetHCapObjectFileInfo(instanceId, ref fileInfo);
    }

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetHCapObjectFrameInfo")]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool InteropGetHCapObjectFrameInfo(int instanceId, ref SVFFrameInfo info);
    public bool GetHCapObjectFrameInfo(ref SVFFrameInfo frameInfo)
    {
        if (s_logstack)
            logger.Log("[INTEROP]GetHCapObjectFrameInfo");
        if (instanceId == InvalidID)
        {
            return false;
        }
        return InteropGetHCapObjectFrameInfo(instanceId, ref frameInfo);
    }

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetSeekRange")]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool InteropGetSeekRange(int instanceId, ref ulong frameStart, ref ulong frameEnd);
    public bool GetSeekRange(ref ulong frameStart, ref ulong frameEnd)
    {
        if (s_logstack)
            logger.Log("[INTEROP]GetSeekRange");
        if (instanceId == InvalidID)
        {
            return false;
        }
        return InteropGetSeekRange(instanceId, ref frameStart, ref frameEnd);
    }

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, EntryPoint = "SeekToFrame")]
    private static extern void InteropSeekToFrame(int instanceId, ulong frameId);
    public void SeekToFrame(ulong frameId)
    {
        if (s_logstack)
            logger.Log("[INTEROP]SeekToFrame");
        if (instanceId == InvalidID)
        {
            return;
        }
        InteropSeekToFrame(instanceId, frameId);
    }

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, EntryPoint = "SetHCapObjectAudio3DPosition")]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool InteropSetHCapObjectAudio3DPosition(int instanceId, float x, float y, float z);
    public bool SetHCapObjectAudio3DPosition(float x, float y, float z)
    {
        if (s_logstack)
            logger.Log("[INTEROP]SetHCapObjectAudio3DPosition");
        if (instanceId == InvalidID)
        {
            return false;
        }
        return InteropSetHCapObjectAudio3DPosition(instanceId, x, y, z);
    }

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, EntryPoint = "SetComputeNormals")]
    private static extern bool InteropSetComputeNormals(int instanceId, [MarshalAs(UnmanagedType.I1)]bool computeNormals);
    public void SetComputeNormals(bool computeNormals)
    {
        if (s_logstack)
            logger.Log("[INTEROP]SetComputeNormals");

        if (instanceId == InvalidID)
        {
            return;
        }
        InteropSetComputeNormals(instanceId, computeNormals);
    }

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetClockScale")]
    [return: MarshalAs(UnmanagedType.R4)]
    private static extern float InteropGetClockScale(int instanceId);
    public float GetClockScale()
    {
        if (instanceId == InvalidID)
        {
            return 1.0f;
        }
        return InteropGetClockScale(instanceId);
    }

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, EntryPoint = "SetClockScale")]
    private static extern void InteropSetClockScale(int instanceId, float scale);
    public void SetClockScale(float scale)
    {
        if (instanceId == InvalidID)
        {
            return;
        }
        InteropSetClockScale(instanceId, scale);
    }

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, EntryPoint = "SetAudioVolume")]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool InteropSetAudioVolume(int instanceId, float volume);
    public bool SetAudioVolume(float volume)
    {
        if (instanceId == InvalidID)
        {
            return false;
        }
        return InteropSetAudioVolume(instanceId, volume);
    }

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, EntryPoint = "EnumerateAudioDevices")]
    private static extern int InteropEnumerateAudioDevices([In, Out] AudioDeviceInfo[] deviceInfos, int deviceInfosCount);
    public static AudioDeviceInfo[] EnumerateAudioDevices()
    {
        var deviceInfos = new AudioDeviceInfo[10];
        var numReturned = InteropEnumerateAudioDevices(deviceInfos, deviceInfos.Length);
        var result = new AudioDeviceInfo[numReturned];
        Array.Copy(deviceInfos, result, result.Length);
        return result;
    }

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, EntryPoint = "EnableTracing")]
    private static extern void InteropEnableTracing([MarshalAs(UnmanagedType.I1)] bool enable, int level);
    public void EnableTracing(bool enable, LogLevel logLevel)
    {
        if (s_logstack)
        {
            logger.Log("[INTEROP]EnableTracing");
        }

        int nativeLoggingLevel = 0; // chatty
        logger.logEnabled = true;
        switch (logLevel)
        {
            case LogLevel.Verbose:
                nativeLoggingLevel = 0; // chatty
                logger.filterLogType = LogType.Log;
                break;
            case LogLevel.Message:
                nativeLoggingLevel = 1; // normal
                logger.filterLogType = LogType.Log;
                break;
            case LogLevel.Debug:
                nativeLoggingLevel = 2; // errors and warnings
                logger.filterLogType = LogType.Warning;
                break;
            case LogLevel.Critical:
                nativeLoggingLevel = 3; // errors
                logger.filterLogType = LogType.Error;
                break;
            case LogLevel.None:
                nativeLoggingLevel = 4; // never
                logger.logEnabled = false;
                break;
        }

        InteropEnableTracing(enable, nativeLoggingLevel);
    }

    [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetNextTraceLine")]
    [return: MarshalAs(UnmanagedType.LPStr)]
    private static extern string InteropGetNextTraceLine();
    private string GetNextTraceLine()
    {
        return InteropGetNextTraceLine();
    }

    public string[] GetTrace()
    {
        if (s_logstack)
            logger.Log("[INTEROP]GetTrace");

        List<string> traceLines = new List<string>();
        while (true)
        {
            string line = GetNextTraceLine();
            if (false == string.IsNullOrEmpty(line))
            {
                traceLines.Add(line);
            }
            else
            {
                break;
            }
        }
        return traceLines.ToArray();
    }

#if UNITY_EDITOR
    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern IntPtr LoadLibrary(string lpFileName);
#endif

    private const int InvalidID = -1;
    private int instanceId = InvalidID;
    public int InstanceId
    {
        get { return instanceId; }
    }

    public SVFUnityPluginInterop()
    {
        instanceId = InvalidID;

#if UNITY_EDITOR
        EnableTracing(true, LogLevel.Debug);
#endif
    }

    private bool isInstanceDisposed = false;
    protected virtual void Dispose(bool disposing)
    {
        if (!isInstanceDisposed)
        {
            if (this.instanceId != InvalidID)
            {
                DestroyHCapObject();
                this.instanceId = InvalidID;
            }
            isInstanceDisposed = true;
        }
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    ~SVFUnityPluginInterop()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(false);
    }
}
