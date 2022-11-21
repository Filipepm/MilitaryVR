using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class ButtonHandPressScript : MonoBehaviour
{
    bool isPressing = false;


    [Header("Material and Mesh Renderer References")]
    Material DefaultMat;
    public Material PressedMat;
    public MeshRenderer Button_MeshRenderer;

    [Header("Unity Events")]
    public UnityEvent OnButtonDown;

    [Header("Transform References")]
    public Vector3 pressDirection = Vector3.down;
    Vector3 defaultPosition;
    Vector3 pressedPosition;

    [Header("Additional Settings")]
    public float buttonReleaseTime = 1f;


    //public float smooth = 0.01f;
    private IEnumerator Press_IEnumerator;

    private AudioSource audioSource;

    public float buttonPressAmount = 0.01f;
    private void Awake()
    {
        defaultPosition = transform.localPosition;
        Debug.Log("Default pos:"+defaultPosition);
        pressedPosition = defaultPosition + (Vector3.down * buttonPressAmount);
        Debug.Log(pressedPosition);
        DefaultMat = Button_MeshRenderer.material;

        audioSource = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isPressing == false)
        {
            if (other.gameObject.tag == "Hand")
            {
                Press_IEnumerator = Press(other);
                StartCoroutine(Press_IEnumerator);
                audioSource.Play();
                Debug.Log("Pressed the button");

            }
        }
    }

    private IEnumerator Press(Collider other)
    {
        isPressing = true;

        Button_MeshRenderer.material = PressedMat;
        transform.localPosition = pressedPosition;

        yield return new WaitForSeconds(buttonReleaseTime);
       

        transform.localPosition = defaultPosition;

        Button_MeshRenderer.material = DefaultMat;
        isPressing = false;
        OnButtonDown.Invoke();
    }

    private void OnDisable()
    {
        transform.localPosition = defaultPosition;

        Button_MeshRenderer.material = DefaultMat;
        isPressing = false;
    }
}
