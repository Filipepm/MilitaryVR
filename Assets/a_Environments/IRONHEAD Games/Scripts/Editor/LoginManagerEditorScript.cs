using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Photon.Pun;

[CustomEditor(typeof(LoginManager),true)]
public class LoginManagerEditorScript : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.HelpBox("This script is responsbile for connecting to Photon servers.",MessageType.Info);

        LoginManager loginManager = (LoginManager)target;
        if (GUILayout.Button("Connect Anonymously"))
        {
            string randomName = "Player" + Random.Range(1,10000);
            PhotonNetwork.LocalPlayer.NickName = randomName;
            loginManager.ConnectAnonymously();
        }

    }


}
