using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System;

public class UI_InteractionController : MonoBehaviour
{
    [SerializeField]
    GameObject UIController;

   

    [SerializeField]
    GameObject BaseController;

    [SerializeField]
    InputActionReference inputActionReference_UISwitcher;

    bool isUICanvasActive = false;


    [SerializeField]
    GameObject UIPanels_Gameobject;

    [SerializeField]
    GameObject AvatarSelectionPlatform_Gameobject;

    [Tooltip("Ignore if you have a static position for UI Panel.")]
    [SerializeField]
    Vector3 positionOffsetForUICanvasGameobject;

    [SerializeField]
    bool alwaysAppearInFront = false;

    private void OnEnable()
    {
        inputActionReference_UISwitcher.action.performed += ActivateUIMode;
    }
    private void OnDisable()
    {
        inputActionReference_UISwitcher.action.performed -= ActivateUIMode;

    }

    private void Start()
    {
        ////Deactivating UI Canvas Gameobject by default
        UIPanels_Gameobject.SetActive(false);

        //Deactivating UI Controller by default
        UIController.GetComponent<XRRayInteractor>().enabled = false;
        UIController.GetComponent<XRInteractorLineVisual>().enabled = false;
    }

    /// <summary>
    /// This method is called when the player presses UI Switcher Button which is the input action defined in Default Input Actions.
    /// When it is called, UI interaction mode is switched on and off according to the previous state of the UI Canvas.
    /// </summary>
    /// <param name="obj"></param>
    private void ActivateUIMode(InputAction.CallbackContext obj)
    {
        if (!isUICanvasActive)
        {
            isUICanvasActive = true;

            //Activating UI Controller by enabling its XR Ray Interactor and XR Interactor Line Visual
            UIController.GetComponent<XRRayInteractor>().enabled = true;
            UIController.GetComponent<XRInteractorLineVisual>().enabled = true;

            //Deactivating Base Controller by disabling its XR Direct Interactor
            BaseController.GetComponent<XRDirectInteractor>().enabled = false;

            if (alwaysAppearInFront)
            {
                //Adjusting the transform of the UI Canvas Gameobject according to the VR Player transform
                Vector3 positionVec = new Vector3(UIController.transform.position.x, positionOffsetForUICanvasGameobject.y, UIController.transform.position.z);
                Vector3 directionVec = UIController.transform.forward;
                directionVec.y = 0f;
                UIPanels_Gameobject.transform.position = positionVec + positionOffsetForUICanvasGameobject.magnitude * directionVec;
                UIPanels_Gameobject.transform.rotation = Quaternion.LookRotation(directionVec);
            }
          

            //Activating the UI Canvas Gameobject
            UIPanels_Gameobject.SetActive(true);

            //Activating the Avatar Selection Platform Gameobject
            if (AvatarSelectionPlatform_Gameobject!=null)
            {
                AvatarSelectionPlatform_Gameobject.SetActive(true);

            }


        }
        else
        {
            isUICanvasActive = false;

            //De-Activating UI Controller by enabling its XR Ray Interactor and XR Interactor Line Visual
            UIController.GetComponent<XRRayInteractor>().enabled = false;
            UIController.GetComponent<XRInteractorLineVisual>().enabled = false;

            //Activating Base Controller by disabling its XR Direct Interactor
            BaseController.GetComponent<XRDirectInteractor>().enabled = true;

            //De-Activating the UI Canvas Gameobject
            UIPanels_Gameobject.SetActive(false);

            //De-Activating the Avatar Selection Platform Gameobject
            if (AvatarSelectionPlatform_Gameobject != null)
            {
                AvatarSelectionPlatform_Gameobject.SetActive(false);

            }
        }

    }

    public void Activate_VRKeyboardMode()
    {
        //De-Activating UI Controller by enabling its XR Ray Interactor and XR Interactor Line Visual
        UIController.GetComponent<XRRayInteractor>().enabled = false;
        UIController.GetComponent<XRInteractorLineVisual>().enabled = false;

        //Activating Base Controller by disabling its XR Direct Interactor
        BaseController.GetComponent<XRDirectInteractor>().enabled = true;
    }

    public void DeActivate_VRKeyboardMode()
    {
        //Activating UI Controller by enabling its XR Ray Interactor and XR Interactor Line Visual
        UIController.GetComponent<XRRayInteractor>().enabled = true;
        UIController.GetComponent<XRInteractorLineVisual>().enabled = true;

        //Deactivating Base Controller by disabling its XR Direct Interactor
        BaseController.GetComponent<XRDirectInteractor>().enabled = false;
    }
}
