using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using IRONHEADGames;
public class HandsAnimationController : MonoBehaviour
{
    [SerializeField] Animator HandAnimator;
    [SerializeField] InputAction gripInputAction;
    [SerializeField] InputAction triggerInputAction;
    [SerializeField] string WhichHand = "";

    private void Awake()
    {
        gripInputAction.performed += GripPressed;
        triggerInputAction.performed += TriggerPressed; 
    }

    private void OnEnable()
    {
        gripInputAction.Enable();
        triggerInputAction.Enable();     
    }

    private void OnDisable()
    {
        gripInputAction.Disable();
        triggerInputAction.Disable();
    }
   
    private void TriggerPressed(InputAction.CallbackContext obj)
    {
        HandAnimator.SetFloat("Trigger_"+WhichHand, obj.ReadValue<float>());
        // Debug.Log("Trigger Pressed " + obj.ReadValue<float>());
    }

    private void GripPressed(InputAction.CallbackContext obj)
    {
        HandAnimator.SetFloat("Grip_"+WhichHand, obj.ReadValue<float>());
        //Debug.Log("Grip Pressed " + obj.ReadValue<float>());
    }
}
