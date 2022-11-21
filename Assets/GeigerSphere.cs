using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeigerSphere : MonoBehaviour
{
    [SerializeField] public GameObject circle;

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Geiger")
        { // only an object tagged Player stops the sound
            circle.SetActive(true);
        }
    }
}
