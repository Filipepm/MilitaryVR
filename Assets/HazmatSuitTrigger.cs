using BNG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazmatSuitTrigger : MonoBehaviour
{
    [SerializeField] public HandModelSelector player;
    public void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
        //player.HandUpdate();
    }
}
