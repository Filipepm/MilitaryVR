using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
public class SpawnManager : MonoBehaviourPunCallbacks
{
    public Transform[] spawnPositions;

    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {   
        SpawnPlayer();
    }
    #endregion


    #region Private Methods
    private void SpawnPlayer()
    {
        int randomSpawnPoint = Random.Range(0, spawnPositions.Length - 1);
        Vector3 randomInstantiatePosition = spawnPositions[randomSpawnPoint].position;

        if (MultiplayerVRConstants.USE_FINALIK)
        {
            PhotonNetwork.Instantiate("NetworkedVRPlayerPrefab_FinalIK", randomInstantiatePosition, Quaternion.identity, 0);

        }else if (MultiplayerVRConstants.USE_FINALIK_UMA2)
        {
            PhotonNetwork.Instantiate("NetworkedVRPlayerPrefab_FinalIK_UMA2", randomInstantiatePosition, Quaternion.identity, 0);
        }
        else
        {
            PhotonNetwork.Instantiate("NetworkedVRPlayerPrefab", randomInstantiatePosition, Quaternion.identity, 0);
        }
    }
    #endregion
}
