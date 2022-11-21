using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;


public class LocalPlayerUIManager : MonoBehaviour
{
    [Tooltip("Health value between 0 and 100.")]
    [SerializeField]
    GameObject goHome_Button;

    [SerializeField]
    TextMeshProUGUI currentRoomName_Text;

    private void Start()
    {
        goHome_Button.GetComponent<Button>().onClick.AddListener(VirtualWorldManager.Instance.LeaveRoomAndLoadHomeScene);

        if (currentRoomName_Text != null && PhotonNetwork.InRoom)
        {
            currentRoomName_Text.text = PhotonNetwork.CurrentRoom.Name;
        }
    }   
}
