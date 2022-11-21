using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using System.IO;

public partial class Build {

    private static int updateVSProjectFiles(string path, string oldValue, string newValue)
    {
        int count = 0;
        string[] csprojLines = File.ReadAllLines(path);
        string trimmedOldValue = oldValue.Trim().ToLower();
        string trimmedCSProjFileLine = "";
        FileStream fs = File.Create(path);

        using (StreamWriter sw = new StreamWriter(fs))
        {
            trimmedCSProjFileLine = "";
            for (int i = 0; i < csprojLines.Length; i++)
            {
                trimmedCSProjFileLine = csprojLines[i].Trim().ToLower();

                if (trimmedCSProjFileLine == trimmedOldValue)
                {
                    sw.WriteLine(newValue);
                    count++;
                }
                else
                {
                    sw.WriteLine(csprojLines[i]);
                }
            }
        }
        if (count == 0)
        {
            Debug.Log("No substitution in " + path + " " + oldValue + " to " + newValue);
        }
        return count;
    }

    private static string WriteManifestExtension()
    {
        string returnXml = "";
        returnXml += "  </Applications>\n";
        returnXml += "  <Extensions>\n";
        returnXml += "    <Extension Category=\"windows.activatableClass.inProcessServer\">\n";
        returnXml += "      <InProcessServer>\n";
        returnXml += "        <Path>SVFRT.dll</Path>\n";
        returnXml += "        <ActivatableClass ActivatableClassId=\"SVF.CH264Wrapper\" ThreadingModel=\"both\" />\n";
        returnXml += "      </InProcessServer>\n";
        returnXml += "    </Extension>\n";
        returnXml += "  </Extensions>";

        return returnXml;
    }

    [PostProcessBuild]
    public static void EditAppxManifest(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (buildTarget == BuildTarget.WSAPlayer)
        {
            var appxManifestPath = Path.Combine(Path.Combine(pathToBuiltProject, PlayerSettings.productName), "Package.appxmanifest");
            Debug.Log("appxManifestPath = " + appxManifestPath);

            if (!File.ReadAllText(appxManifestPath).Contains("SVFRT.dll"))
            {
                updateVSProjectFiles(appxManifestPath, "</applications>", WriteManifestExtension());
            }
        }
    }
}