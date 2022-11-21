using System.IO;
using UnityEditor;

[InitializeOnLoad]
public class EditorClearPreviewOnStartup
{
    // Questionable hack: InitializeOnLoad is actually invoked every single time the editor
    // window comes into focus, and there is no close equivalent to check if the editor is
    // being launched for the first time. The timeSinceStartup value is actually based on the
    // editor instance, not the opening of the project, so if we take more than a minute to
    // select and open the project, this will not get executed. But it's probably better than
    // generating a preview frame for the rest of the session.
    private readonly static double CLEAR_PREVIEW_TIMEOUT = 60;

    static EditorClearPreviewOnStartup()
    {
        string previewDir = "Assets/SVFUnityPlugin/PreviewAssets";
        if (EditorApplication.timeSinceStartup > CLEAR_PREVIEW_TIMEOUT || !Directory.Exists(previewDir))
        {
            return;
        }
        foreach (string path in Directory.GetFiles(previewDir))
        {
            if (Path.GetExtension(path) == ".asset")
            {
                FileUtil.DeleteFileOrDirectory(path);
            }
        }
    }
}
