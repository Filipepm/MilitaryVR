using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using System.IO;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;

public partial class Build {

    [PostProcessBuild]
    public static void AddFrameworks(BuildTarget buildTarget, string pathToBuiltProject) {

        if (buildTarget == BuildTarget.iOS) {

            string projectPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);

            PBXProject project = new PBXProject();
            project.ReadFromString(File.ReadAllText(projectPath));
            string targetName = PBXProject.GetUnityTargetName();
            string targetGUID = project.TargetGuidByName(targetName);
            project.AddFrameworkToProject(targetGUID, "VideoToolbox.framework", false);

            File.WriteAllText(projectPath, project.WriteToString());
        }
    }
}
#endif