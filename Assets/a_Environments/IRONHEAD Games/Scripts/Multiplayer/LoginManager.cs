using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class LoginManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField PlayerName_InputField;
    

    #region UI Callback Methods
    public void ConnectToPhotonServer()
    {
        if (PlayerName_InputField != null)
        {
            PhotonNetwork.NickName = PlayerName_InputField.text;
            PhotonNetwork.ConnectUsingSettings();
            DebugUIManager.instance.ShowDebugUIMessage("Connecting...");
        }

       
    }

    public void ConnectAnonymously()
    {
        PhotonNetwork.ConnectUsingSettings();
        DebugUIManager.instance.ShowDebugUIMessage("Connecting...");
    }
    #endregion

    #region Photon Callback Methods
    public override void OnConnected()
    {
        Debug.Log("OnConnected is called. The server is available.");
        DebugUIManager.instance.ShowDebugUIMessage("Connected to server...");

    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to the Master Server with player name: "+PhotonNetwork.NickName);
        DebugUIManager.instance.FinishDebugUIMessage();

       
        if (MultiplayerVRConstants.USE_FINALIK)
        {
            //SceneLoader.instance.LoadScene("HomeScene_FinalIK", false);

        }else if (MultiplayerVRConstants.USE_FINALIK_UMA2)
        {
            //SceneLoader.instance.LoadScene("HomeScene_FinalIK_UMA2", false);
        }
        else
        {
            //SceneLoader.instance.LoadScene("HomeScene", false);
            Debug.Log("Yay");
        }
    }


    #endregion

}
