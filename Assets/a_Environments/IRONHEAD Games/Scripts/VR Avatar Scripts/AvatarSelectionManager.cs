using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarSelectionManager : MonoBehaviour
{
    [SerializeField]
    GameObject AvatarSelectionPlatformGameobject;

    [SerializeField]
    GameObject VRPlayerGameobject;


    public GameObject[] selectableAvatarModels;
    public GameObject[] loadableAvatarModels;

    public int selectedAvatarIndex = 0;

    public AvatarInputConverter avatarInputConverter;


    public static AvatarSelectionManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        //Initially, de-activating the Avatar Selection Platform.
        AvatarSelectionPlatformGameobject.SetActive(false);

        //Checking if we already have the avatar selection data so that the corrrect avatar models can be instantiated.
        //This will be useful when returning back from networked scenes into homescene.
        object avatarSelectionNumber;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.SELECTED_AVATAR_INDEX, out avatarSelectionNumber))
        {
            Debug.Log((int)avatarSelectionNumber);
            selectedAvatarIndex = (int)avatarSelectionNumber;
            ActivateAvatarModelAt(selectedAvatarIndex);
            LoadAvatarModelAt(selectedAvatarIndex);
        }
        else
        {
            //If we do not have any selected Avatar, we simply use the default one which is the first Avatar in the SelectableAvatarModels list.
            selectedAvatarIndex = 0;
            ActivateAvatarModelAt(selectedAvatarIndex);
            LoadAvatarModelAt(selectedAvatarIndex);
        }
    }

    public void NextAvatar()
    {
        selectedAvatarIndex += 1;
        if (selectedAvatarIndex >= selectableAvatarModels.Length)
        {
            selectedAvatarIndex = 0;
        }
        ActivateAvatarModelAt(selectedAvatarIndex);

    }

    public void PreviousAvatar()
    {
        selectedAvatarIndex -= 1;

        if (selectedAvatarIndex < 0)
        {
            selectedAvatarIndex = selectableAvatarModels.Length - 1;
        }
        ActivateAvatarModelAt(selectedAvatarIndex);
        
    }

    private void ActivateAvatarModelAt(int avatarIndex)
    {
        foreach (GameObject selectableAvatarModel in selectableAvatarModels)
        {
            selectableAvatarModel.SetActive(false);
        }

        selectableAvatarModels[avatarIndex].SetActive(true);
        Debug.Log(selectedAvatarIndex);
        LoadAvatarModelAt(selectedAvatarIndex);
    }

    private void LoadAvatarModelAt(int avatarIndex)
    {
        foreach (GameObject loadableAvatarModel in loadableAvatarModels)
        {
            loadableAvatarModel.SetActive(false);
        }
        loadableAvatarModels[avatarIndex].SetActive(true);

        avatarInputConverter.MainAvatarTransform = loadableAvatarModels[avatarIndex].GetComponent<AvatarHolder>().MainAvatarTransform;

        avatarInputConverter.AvatarBody = loadableAvatarModels[avatarIndex].GetComponent<AvatarHolder>().BodyTransform;
        avatarInputConverter.AvatarHead = loadableAvatarModels[avatarIndex].GetComponent<AvatarHolder>().HeadTransform;
        avatarInputConverter.AvatarHand_Left = loadableAvatarModels[avatarIndex].GetComponent<AvatarHolder>().HandLeftTransform;
        avatarInputConverter.AvatarHand_Right = loadableAvatarModels[avatarIndex].GetComponent<AvatarHolder>().HandRightTransform;

        //Setting up avatar selection as custom player property in Photon
        ExitGames.Client.Photon.Hashtable playerSelectionProp = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.SELECTED_AVATAR_INDEX, selectedAvatarIndex } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProp);
    }
}
