using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;

public class TransitionArea : MonoBehaviour
{
    [SerializeField] public Transform teleportCockpit;
    [SerializeField] public Transform teleportAirport;
    [SerializeField] public GameObject player;
    [SerializeField] public GameObject playerp;

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            playerp.transform.position = teleportCockpit.transform.position;
            playerp.transform.rotation = teleportCockpit.transform.rotation;
            playerp.GetComponent<PlayerGravity>().enabled = false;
            playerp.GetComponent<LocomotionManager>().enabled = false;
            playerp.GetComponent<CharacterController>().enabled = false;

        }
    }

    public void TeleportAirport()
    {
        playerp.transform.position = teleportAirport.transform.position;
        playerp.transform.rotation = teleportAirport.transform.rotation;
        playerp.GetComponent<PlayerGravity>().enabled = true;
        playerp.GetComponent<LocomotionManager>().enabled = true;
        playerp.GetComponent<CharacterController>().enabled = true;
    }
}
