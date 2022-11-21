using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using BNG;

    /// <summary>
    /// This script will toggle a GameObject whenever the provided InputAction is executed
    /// </summary>
public class Firehose : MonoBehaviour
{
    private ControllerBinding ToggleHandsInputR = ControllerBinding.RightTriggerDown;
    private ControllerBinding ToggleHandsInputL = ControllerBinding.LeftTriggerDown;
    public InputActionReference InputAction = default;
    public ParticleSystem ToggleObject = default;
    public bool toggled = false;
    public Grabber rightGrab;
    public Grabber leftGrab;
    private Grabbable grabl;
    private Grabbable grabr;

    private void Start()
    {
        ToggleObject.Stop();

    }

    public void Update()
    {
        if ((ToggleHandsInputL.GetDown() || ToggleHandsInputR.GetDown()) && (rightGrab.HoldingItem || leftGrab.HoldingItem))
        {

            //grabr = rightGrab.HoldingSomething();
            //grabl = leftGrab.HoldingSomething();
            if (grabr != null)
            {
                if (grabr.name.ToString() == "Fire_Hose")
                {
                    ToggleActive();
                }
            }

            if (grabl != null)
            {
                if (grabl.name.ToString() == "Fire_Hose")
                {
                    ToggleActive();
                }
            }
        }

        if((rightGrab.HoldingItem || leftGrab.HoldingItem) == false)
        {
            ToggleObject.Stop();
            toggled = false;
        }
    }

    public void ToggleActive()
    {
        if (toggled == false)
        {
            ToggleObject.Play();
            toggled = true;
        }
        else
        {
            ToggleObject.Stop();
            toggled = false;
        } 
    }
}

