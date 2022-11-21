using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
   
    public static SceneLoader instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private string sceneNameToBeLoaded;
    private bool isSceneSynched = false;

    public void LoadScene(string _sceneName, bool _isSceneSynched)
    {
        sceneNameToBeLoaded = _sceneName;
        isSceneSynched = _isSceneSynched;

        StartCoroutine(InitializeSceneLoading());
    }



    IEnumerator InitializeSceneLoading()
    {
        //First, we load the Loading scene
        yield return //SceneManager.LoadSceneAsync("LoadingScene");

        //Load the actual scene
        StartCoroutine(ShowOverlayAndLoad());
    }

    /// <summary>
    /// If isSceneSynch is true, this coroutine loads the scene with PhotonNetwork.LoadLevel method.
    /// PhotonNetwork.LoadLevel is used when synchronizing the scenes.
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    IEnumerator ShowOverlayAndLoad()
    {
        //Waiting some seconds to prevent "pop" to new scene
        yield return new WaitForSeconds(4f);

        if (isSceneSynched)
        {
            //If Scene should be loaded as a Multiplayer scene, use PhotonNetwork.LoadLevel
            PhotonNetwork.LoadLevel(sceneNameToBeLoaded);
        }
        else
        {
            //If it is a local scene loading, load the scene locally.
            //Load Scene and wait until complete
            AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneNameToBeLoaded);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            yield return null;
        }      
    }
}
