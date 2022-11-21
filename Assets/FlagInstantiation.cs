using BNG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagInstantiation : MonoBehaviour
{
    public ControllerBinding ToggleHandsInput = ControllerBinding.RightThumbstickDown;
    public GameObject[] flagArray = new GameObject[2];
    private int currentIndex = 0;
    private bool toggled = false;
    private GameObject instantiatedObject;
    private Vector3 playerPos;
    private Quaternion playerRot;
    public GameObject player;
    public int facts23;

    public void Update()
    {
        if (ToggleHandsInput.GetDown())
        {
            playerPos = player.gameObject.transform.position;
            playerRot = player.transform.localRotation;
            FlagPrefab();
        }
    }

    public void ChangeIndex()
    {
        currentIndex++;
        toggled = false;
    }

    public void FlagPrefab()
    {
        if (toggled == false)
        {
            instantiatedObject = Instantiate(flagArray[currentIndex], new Vector3(playerPos.x + facts23, playerPos.y, playerPos.z), Quaternion.identity);
            toggled = true;
        }
        else
        {
            Destroy(instantiatedObject);
            toggled = false;
        }
    }
}
