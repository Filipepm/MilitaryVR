using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class PlayerNetworkSetup : MonoBehaviourPun
{

    public GameObject localXRRigGameobject;//XR Rig
    public GameObject mainAvatarGamebject;

    public GameObject avatarHeadGameobject;
    public GameObject avatarBodyGameobject;

    public TextMeshProUGUI playerName_Text;
    public GameObject[] AvatarModelPrefabs;

    [SerializeField]
    GameObject nonNetworkedGameobjects;

   

    // Start is called before the first frame update
    void Awake()
    {
        if (photonView.IsMine)
        {
            //If this is the case, the player is the local player
            localXRRigGameobject.SetActive(true);
            mainAvatarGamebject.SetActive(true);


            //Getting the avatar selection data so that the corrrect avatar models can be instantiated.
            object avatarSelectionNumber;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.SELECTED_AVATAR_INDEX, out avatarSelectionNumber))
            {
                Debug.Log((int)avatarSelectionNumber);
                photonView.RPC("InitializeSelectedAvatarModel", RpcTarget.AllBuffered, (int)avatarSelectionNumber);
            }
            //Setting the layer of avatar head to AvatarLocalHead layer so that it does not block the view of the local VR Player
            SetLayerRecursively(avatarHeadGameobject, 11);

            TeleportationArea[] teleportationAreas = GameObject.FindObjectsOfType<TeleportationArea>();
            if (teleportationAreas.Length > 0)
            {
                Debug.Log("Found " + teleportationAreas.Length + " teleportation arena");
                foreach (var item in teleportationAreas)
                {
                    item.teleportationProvider = localXRRigGameobject.GetComponent<TeleportationProvider>();
                }
            }
            mainAvatarGamebject.AddComponent<AudioListener>();

            //If the player is local, the player should have the option to leave the room. 
            //This is possible with UI Menu gameobject under Non-networked Gameobjects under NetworkedPlayerPrefab
            //But this operation must be done only locally.
            //That is why we enable LocalPlayerUIManager component for the local player
            nonNetworkedGameobjects.SetActive(true);


        }
        else
        {
            //Else, the player is the remote player
            //So, certain actions must be done
            //For example, XRRig will be disabled for remote players
            localXRRigGameobject.SetActive(false);
            mainAvatarGamebject.SetActive(true);

            //Disabling AvatarInputConverter script
            //Because we do not need it in remote players' game
            mainAvatarGamebject.GetComponent<AvatarInputConverter>().enabled = false;

           
            //Remote players can be seen by the local player
            //So we set Avatar head and body layer to Default layer
            SetLayerRecursively(avatarHeadGameobject, 0);
            SetLayerRecursively(avatarBodyGameobject, 0);

            //De-activating non-networked gameobjects such as Voice Debug UI and UI Menu.
            //These gameobjects will be activated only locally so there is no need them to be active in remote players.
            nonNetworkedGameobjects.SetActive(false);
        }

        //Setting up Player name. Can be configured for player health, too.
        SetAvatarUI();

    }

    [PunRPC]
    public void InitializeSelectedAvatarModel(int avatarSelectionNumber)
    {
        GameObject selectedAvatarGameobject = Instantiate(AvatarModelPrefabs[avatarSelectionNumber], localXRRigGameobject.transform);

        AvatarInputConverter avatarInputConverter = transform.GetComponentInChildren<AvatarInputConverter>();
        AvatarHolder avatarHolder = selectedAvatarGameobject.GetComponent<AvatarHolder>();
        SetUpAvatarGameobject(avatarHolder.HeadTransform, avatarInputConverter.AvatarHead);
        SetUpAvatarGameobject(avatarHolder.BodyTransform, avatarInputConverter.AvatarBody);
        SetUpAvatarGameobject(avatarHolder.HandLeftTransform, avatarInputConverter.AvatarHand_Left);
        SetUpAvatarGameobject(avatarHolder.HandRightTransform, avatarInputConverter.AvatarHand_Right);
    }

    void SetUpAvatarGameobject(Transform avatarModelTransform, Transform parentAvatarTransform)
    {
        avatarModelTransform.SetParent(parentAvatarTransform);
        avatarModelTransform.localPosition = Vector3.zero;
        avatarModelTransform.localRotation = Quaternion.identity;
    }

    void SetAvatarUI()
    {
        playerName_Text.text = photonView.Owner.NickName;
    }


    void SetLayerRecursively(GameObject go, int layerNumber)
    {
        if (go == null) return;
        foreach (Transform trans in go.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = layerNumber;
        }
    }


}
