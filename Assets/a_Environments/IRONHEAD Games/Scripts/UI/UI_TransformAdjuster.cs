using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_TransformAdjuster : MonoBehaviour
{
    [SerializeField]
    public GameObject VRPlayer_Gameobject;

    [SerializeField]
    public GameObject UItoAdjust_Gameobject;

    [Tooltip("Ignore if you have a static position for UI Panel.")]
    [SerializeField]
    Vector3 positionOffsetForUICanvasGameobject;
    // Start is called before the first frame update
    void Start()
    {
        //Adjusting the transform of the UI Canvas Gameobject according to the VR Player transform
        Vector3 positionVec = new Vector3(VRPlayer_Gameobject.transform.position.x, positionOffsetForUICanvasGameobject.y, VRPlayer_Gameobject.transform.position.z);
        Vector3 directionVec = VRPlayer_Gameobject.transform.forward;
        directionVec.y = 0f;
        UItoAdjust_Gameobject.transform.position = positionVec + positionOffsetForUICanvasGameobject.magnitude * directionVec;
        UItoAdjust_Gameobject.transform.rotation = Quaternion.LookRotation(directionVec);
    }

   
}
