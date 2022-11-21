#if UNITY_EDITOR
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

[ExecuteInEditMode]
[RequireComponent(typeof(HoloVideoObject))]
public class HoloVideoPreview : MonoBehaviour
{
    [Tooltip("Usually okay to leave at 1, but try 2 if your first frame's drawing a blank")]
    public uint previewFrame = 2;

    [SerializeField]
    private HoloVideoObject previewVideo;

    private string previewAssetBasePath = "Assets/SVFUnityPlugin/PreviewAssets/";

    void Start()
    {
        Directory.CreateDirectory(previewAssetBasePath);

        previewVideo = GetComponent<HoloVideoObject>();

        // Make sure this is a child of the HoloVideoObject and its transformation matches its parent;
        HoloVideoObject realVideo = transform.parent.GetComponent<HoloVideoObject>();
        previewVideo.Url = realVideo.Url;
        Assert.IsNotNull(realVideo);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        // In editor play mode, we play a video and pause fix it at given preview frame
        if (EditorApplication.isPlaying)
        {
            previewVideo.Open(previewVideo.Url);
            previewVideo.Play();
            previewVideo.DisplayFrame(previewFrame);
            StartCoroutine(CaptureFrameWhenPaused());
        }
        // In the edit mode, we instead grab our saved assets and assign them to the preview object
        else
        {
            Mesh previewMesh = (Mesh) AssetDatabase.LoadAssetAtPath(GetPreviewAssetPath("mesh"), typeof(Mesh));
            if (previewMesh != null)
            {
                GetComponent<MeshFilter>().mesh = previewMesh;
            }
            Texture2D previewTexture = (Texture2D) AssetDatabase.LoadAssetAtPath(GetPreviewAssetPath("texture"), typeof(Texture2D));
            if (previewTexture != null)
            {
                GetComponent<Renderer>().sharedMaterial.mainTexture = previewTexture;
                GetComponent<Renderer>().sharedMaterial.SetTexture("_EmissionMap", previewTexture);
            }
        }
        previewVideo.GetComponent<MeshRenderer>().enabled = !EditorApplication.isPlaying;
    }

    IEnumerator CaptureFrameWhenPaused()
    {
        while (previewVideo.GetCurrentState() != SVFPlaybackState.Paused)
        {
            yield return null;
        }
        yield return null;
        Mesh previewMesh = (Mesh) AssetDatabase.LoadAssetAtPath(GetPreviewAssetPath("mesh"), typeof(Mesh));
        if (previewMesh == null)
        {
            AssetDatabase.CreateAsset(previewVideo.GetComponent<MeshFilter>().mesh, GetPreviewAssetPath("mesh"));
        }
        Texture2D previewTexture = (Texture2D) AssetDatabase.LoadAssetAtPath(GetPreviewAssetPath("texture"), typeof(Texture2D));
        if (previewTexture == null)
        {
            AssetDatabase.CreateAsset(previewVideo.GetComponent<Renderer>().material.mainTexture, GetPreviewAssetPath("texture"));
        }
    }

    private string GetPreviewAssetPath(string assetType)
    {
        if (assetType == "mesh")
        {
            return previewAssetBasePath + Path.GetFileNameWithoutExtension(previewVideo.Url) + "_previewMesh.asset";
        }
        else if (assetType == "texture")
        {
            return previewAssetBasePath + Path.GetFileNameWithoutExtension(previewVideo.Url) + "_previewTexture.asset";
        }
        else
        {
            Assert.IsTrue(false);
            return "";
        }
    }
}
#endif
