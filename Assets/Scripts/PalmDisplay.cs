using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;
public class PalmDisplay : MonoBehaviour
{
    [SerializeField] private GameObject hud;
    private Grabber _grabber;

    private void Start()
    {
        _grabber = GetComponentInChildren<Grabber>();
    }

    private void Update()
    {
        var rotation = transform.rotation.eulerAngles;
        var xrange = rotation.x > -12f && rotation.x < 12f;
        var zrange = rotation.z > 70f && rotation.z < 110f;

        if(xrange && zrange && !_grabber.HoldingItem)
        {
            hud.SetActive(true);
        }
        else
        {
            hud.SetActive(false);
        }
    }
}
