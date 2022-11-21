using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

namespace IRONHEADGames
{
    public class RoomManager : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        TextMeshProUGUI OccupancyRateText_ForSchool;


        [SerializeField]
        TextMeshProUGUI OccupancyRateText_ForOutdoor;

        string mapType;

        #region Unity Methods
        // Start is called before the first frame update
        void Start()
        {
            PhotonNetwork.AutomaticallySyncScene = true;

            if (!PhotonNetwork.IsConnectedAndReady)
            {
                PhotonNetwork.ConnectUsingSettings();
            }
            else
            {
                PhotonNetwork.JoinLobby();
            }
        }
        #endregion


        #region UI Callback Methods
        public void JoinRandomRoom()
        {
            PhotonNetwork.JoinRandomRoom();
        }


        public void OnEnterRoomButtonClicked_Outdoor()
        {
            mapType = MultiplayerVRConstants.MAP_TYPE_VALUE_OUTDOOR;
            ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.MAP_TYPE_KEY, mapType } };
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
            DebugUIManager.instance.ShowDebugUIMessage("Finding Room...");
        }

        public void OnEnterRoomButtonClicked_School()
        {
            mapType = MultiplayerVRConstants.MAP_TYPE_VALUE_SCHOOL;
            ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.MAP_TYPE_KEY, mapType } };
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
            DebugUIManager.instance.ShowDebugUIMessage("Finding Room...");


        }
        #endregion

        #region Photon Callback Methods
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log(message);
            DebugUIManager.instance.ShowDebugUIMessage(message);

            //If the user tries to join the open worlds but, initially, if there is not room at all
            //Then, we create and join a random room with the selected map.
            CreateAndJoinRoom();
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log(message);
            DebugUIManager.instance.ShowDebugUIMessage(message);

        }


        public override void OnCreatedRoom()
        {
            Debug.Log("A room is created with the name: " + PhotonNetwork.CurrentRoom.Name);
            DebugUIManager.instance.ShowDebugUIMessage("Joining Room...");
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected to servers again.");
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedRoom()
        {

            Debug.Log("The local player: " + PhotonNetwork.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name + " Player count " + PhotonNetwork.CurrentRoom.PlayerCount);
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(MultiplayerVRConstants.MAP_TYPE_KEY))
            {
                object mapType;
                if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(MultiplayerVRConstants.MAP_TYPE_KEY, out mapType))
                {
                    Debug.Log("Joined room with the map: " + (string)mapType);
                    if ((string)mapType == MultiplayerVRConstants.MAP_TYPE_VALUE_SCHOOL)
                    {
                        //Load the School scene                
                        DebugUIManager.instance.FinishDebugUIMessage();
                        SceneLoader.instance.LoadScene("World_School", true);

                    }
                    else if ((string)mapType == MultiplayerVRConstants.MAP_TYPE_VALUE_OUTDOOR)
                    {
                        //Load the Outdoor Scene                  
                        DebugUIManager.instance.FinishDebugUIMessage();
                        SceneLoader.instance.LoadScene("World_Outdoor", true);
                    }
                }
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {

            Debug.Log(newPlayer.NickName + " joined to:" + "Player Count: " + PhotonNetwork.CurrentRoom.PlayerCount);
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            if (roomList.Count == 0)
            {
                //There is no room at all.
                OccupancyRateText_ForSchool.text = 0 + " / " + 20;
                OccupancyRateText_ForOutdoor.text = 0 + " / " + 20;
            }

            foreach (RoomInfo room in roomList)
            {
                Debug.Log(room.Name);
                if (room.Name.Contains(MultiplayerVRConstants.MAP_TYPE_VALUE_OUTDOOR))
                {
                    //Update the Outdoor room map
                    Debug.Log("Room is an OUTDOOR map. Player count is: " + room.PlayerCount);
                    OccupancyRateText_ForOutdoor.text = room.PlayerCount + " / " + 20;

                }
                else if (room.Name.Contains(MultiplayerVRConstants.MAP_TYPE_VALUE_SCHOOL))
                {
                    //Update the School room map
                    Debug.Log("Room is a SCHOOL map. Player count is: " + room.PlayerCount);

                    OccupancyRateText_ForSchool.text = room.PlayerCount + " / " + 20;
                }

            }
        }


        public override void OnJoinedLobby()
        {
            Debug.Log("Joined to Lobby");
        }

        #endregion


        #region Public Methods
        public void CreatePrivateRoom(string roomName, byte maxPlayer, string mapType)
        {
            RoomOptions roomOptions = new RoomOptions { MaxPlayers = maxPlayer, PlayerTtl = 1000 };
            string[] roomPropsInLobby = { MultiplayerVRConstants.MAP_TYPE_KEY };
            //Currently, there are 2 different maps: Outdoor and School
            //1.Outdoor = "outdoor"
            //2.School = "school"
            ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.MAP_TYPE_KEY, mapType } };

            //Defining the Map of the room as the custom property.
            //With this way, we can get the Map info inside OnJoinedRoom callback method without the need of keeping this data locally.
            roomOptions.CustomRoomProperties = customRoomProperties;

            //Made the room as invisible so that the room can be private.
            //By doing this, private rooms can be joined only by entering the room name which acts like a password
            roomOptions.IsVisible = false;

            PhotonNetwork.CreateRoom(roomName, roomOptions, null);
        }

        public void JoinPrivateRoom(string roomName)
        {
            if (roomName != null)
            {
                PhotonNetwork.JoinRoom(roomName);
            }
        }

        #endregion

        #region Private Methods
        void CreateAndJoinRoom()
        {
            string randomRoomName = "Room_" + mapType + Random.Range(0, 10000);
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 8;

            string[] roomPropsInLobby = { MultiplayerVRConstants.MAP_TYPE_KEY };
            //There are 2 different maps: Outdoor and School
            //1.Outdoor = "outdoor"
            //2.School = "school"

            ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.MAP_TYPE_KEY, mapType } };

            roomOptions.CustomRoomPropertiesForLobby = roomPropsInLobby;
            roomOptions.CustomRoomProperties = customRoomProperties;


            PhotonNetwork.CreateRoom(randomRoomName, roomOptions);
        }

        #endregion
    }
}


