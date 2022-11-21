using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DebugUIManager : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI debugUI_Text;

    [SerializeField]
    GameObject debugUI_Gameobject;

    public Vector3 relativePositionToAppear;

    public static DebugUIManager instance;

    public float distance = 1f;

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

    private void Start()
    {
       // Debug.Log(Application.persistentDataPath + MultiplayerVRConstants.AVATAR_SAVE_FILE_NAME);
        if (!debugUI_Gameobject)
        {
            debugUI_Gameobject = gameObject;
        }
        debugUI_Gameobject.SetActive(false);
        
    }

    public void ShowDebugUIMessage(string message)
    {
        StopAllCoroutines();
        debugUI_Text.text = message;
        
        debugUI_Gameobject.transform.position = Camera.main.transform.position + Camera.main.transform.forward * distance;
        debugUI_Gameobject.transform.LookAt(Camera.main.transform);


        debugUI_Gameobject.SetActive(true);
        StartCoroutine(FinishDebufMessageAfterSec(2f));

    }

    IEnumerator FinishDebufMessageAfterSec(float sec)
    {

        yield return new WaitForSeconds(sec);

        debugUI_Gameobject.SetActive(false);
    }

    public void FinishDebugUIMessage()
    {
        debugUI_Gameobject.SetActive(false);

    }

}
