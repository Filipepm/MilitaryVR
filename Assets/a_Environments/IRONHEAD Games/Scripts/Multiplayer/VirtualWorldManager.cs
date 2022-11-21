using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events;
public class VirtualWorldManager : MonoBehaviourPunCallbacks
{
    public UnityEvent OnNewPlayerEntered;


    public static VirtualWorldManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance!=this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }


    public void LeaveRoomAndLoadHomeScene()
    {
        PhotonNetwork.LeaveRoom();
    }


    #region Photon Callback Methods
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {

        Debug.Log(newPlayer.NickName + " joined to:" + "Player Count: " + PhotonNetwork.CurrentRoom.PlayerCount);
        OnNewPlayerEntered.Invoke();


    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnJoinedRoom()
    {
        //When joining room, we need to finish showing Debug UI Message.
        DebugUIManager.instance.FinishDebugUIMessage();

    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (MultiplayerVRConstants.USE_FINALIK)
        {
            SceneLoader.instance.LoadScene("Hangar", false);

        }else if (MultiplayerVRConstants.USE_FINALIK_UMA2)
        {
            SceneLoader.instance.LoadScene("Hangar", false);
        }
        else
        {
            SceneLoader.instance.LoadScene("Hangar", false);
        }
    }
    #endregion
}
