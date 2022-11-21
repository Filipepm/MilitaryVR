using BNG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagTrigger : MonoBehaviour
{
    [SerializeField] public FlagInstantiation player;
    public void OnTriggerEnter(Collider other)
    {
        player.enabled = true;
        Destroy(gameObject);
    }
}
