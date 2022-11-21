using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRRigFollower : MonoBehaviour
{

    [SerializeField]
    GameObject xrRig_Gameobject;

    [SerializeField]
    GameObject fullbodyAvatar_Gameobject;

    private void Update()
    {
        fullbodyAvatar_Gameobject.transform.localPosition = Vector3.Lerp(fullbodyAvatar_Gameobject.transform.localPosition,xrRig_Gameobject.transform.localPosition,0.5f);
    }


}
