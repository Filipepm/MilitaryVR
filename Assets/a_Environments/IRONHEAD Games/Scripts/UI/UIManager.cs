using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IRONHEADGames;
using TMPro;
public class UIManager : MonoBehaviour
{
    [Header("Main UI Panel")]
    [SerializeField]
    GameObject MainUIPanel_Gameobject;


    [Header("Create Room UI Panel")]
    [SerializeField]
    GameObject CreateRoomUIPanel_Gameobject;

    [SerializeField]
    GameObject Background_CreateRoomUIPanel_Gameobject;

    public TMP_InputField RoomName_CreateRoom_InputField;
    public TMP_InputField MaxPlayers_CreateRoom_InputField;

    [SerializeField]
    Sprite Map_School_Sprite, Map_Nature_Sprite;

    [Header("Join Rooms UI Panel")]
    [SerializeField]
    GameObject JoinRoomsUIPanel_Gameobject;

    [Header("Join Private Room UI Panel")]
    [SerializeField]
    GameObject JoinPrivateRoomUIPanel_Gameobject;

    public TMP_InputField RoomName_JoinRoom_InputField;


    [Header("Join Open Rooms UI Panel")]
    [SerializeField]
    GameObject JoinOpenRoomsUIPanel_Gameobject;

    [Header("Choose Map UI Panel")]
    [SerializeField]
    GameObject ChooseMapUIPanel_Gameobject;


    public RoomManager RoomManagerScriptRef;
    private string selectedMapType = null;

    #region Unity Methods
    private void Awake()
    {
        //Activating the Main UI Panel 
        ActivatePanel(MainUIPanel_Gameobject);


    }
    #endregion

    #region Public Methods
    public void ActivatePanel(GameObject panelToBeActivated)
    {
        Debug.Log("Activating panel..");
        string panelNameToBeActivated = panelToBeActivated.name;
        MainUIPanel_Gameobject.SetActive(panelNameToBeActivated.Equals(MainUIPanel_Gameobject.name));
        CreateRoomUIPanel_Gameobject.SetActive(panelNameToBeActivated.Equals(CreateRoomUIPanel_Gameobject.name));
        JoinRoomsUIPanel_Gameobject.SetActive(panelNameToBeActivated.Equals(JoinRoomsUIPanel_Gameobject.name));
        JoinPrivateRoomUIPanel_Gameobject.SetActive(panelNameToBeActivated.Equals(JoinPrivateRoomUIPanel_Gameobject.name));
        ChooseMapUIPanel_Gameobject.SetActive(panelNameToBeActivated.Equals(ChooseMapUIPanel_Gameobject.name));
        JoinOpenRoomsUIPanel_Gameobject.SetActive(panelNameToBeActivated.Equals(JoinOpenRoomsUIPanel_Gameobject.name));


    }

    public void Change_UIBackground(string mapName)
    {
        if (mapName == MultiplayerVRConstants.MAP_TYPE_VALUE_SCHOOL)
        {
            //Change background of the Create Room UI Panel
            Background_CreateRoomUIPanel_Gameobject.GetComponent<Image>().sprite = Map_School_Sprite;
            selectedMapType = MultiplayerVRConstants.MAP_TYPE_VALUE_SCHOOL;


        }else if (mapName == MultiplayerVRConstants.MAP_TYPE_VALUE_OUTDOOR)
        {
            //Change background of the Create Room UI Panel
            Background_CreateRoomUIPanel_Gameobject.GetComponent<Image>().sprite = Map_Nature_Sprite;
            selectedMapType = MultiplayerVRConstants.MAP_TYPE_VALUE_OUTDOOR;
        }
    }

    public void OnCreateRoomButtonClicked()
    {
        //Getting the room name from the input field
        
        string roomName = RoomName_CreateRoom_InputField.text;
        roomName = (roomName.Equals(string.Empty)) ? "Room " + Random.Range(1000, 10000) : roomName;


        byte maxPlayers;
        //Max. 8 players is the best for performance
        maxPlayers = (byte)8;
        if (selectedMapType !=null)
        {
            //RoomManagerScriptRef.CreatePrivateRoom(roomName, maxPlayers, selectedMapType);

        }
        else
        {
            DebugUIManager.instance.ShowDebugUIMessage("Please select a map");

        }
    }

    public void OnJoinRoomButtonClicked()
    {
        //Getting the room name from the input field
        string roomName = RoomName_JoinRoom_InputField.text;
        if (roomName != null)
        {
            //RoomManagerScriptRef.JoinPrivateRoom(roomName);

        }
    }
    #endregion
}
