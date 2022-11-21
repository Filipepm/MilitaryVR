using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPressed : MonoBehaviour
{
    [SerializeField] Button createRoom;
    [SerializeField] Button privateRoom;
    [SerializeField] Button publicRoom;
    [SerializeField] Color buttonNotPressed;
    [SerializeField] Color buttonPressed;
    private ColorBlock notPressed;
    private ColorBlock pressed;
    int stage;

    private void Awake()
    {
        notPressed = GetComponent<Button>().colors;
        pressed = GetComponent<Button>().colors;
    }

    public void ButtonPress()
    {


        switch (stage)
        {
            default:
                ButtonNotPressed(createRoom);
                ButtonNotPressed(privateRoom);
                ButtonNotPressed(publicRoom);
                break;

            case 1:
                ButtonPressedd(createRoom);
                ButtonNotPressed(privateRoom);
                ButtonNotPressed(publicRoom);
                break;

            case 2:
                ButtonNotPressed(createRoom);
                ButtonPressedd(privateRoom);
                ButtonNotPressed(publicRoom);
                break;

            case 3:
                ButtonNotPressed(createRoom);
                ButtonNotPressed(privateRoom);
                ButtonPressedd(publicRoom);
                break;
        }
    }

    public void ButtonNotPressed(Button theButton)
    {
        notPressed.normalColor = buttonNotPressed;
        theButton.colors = notPressed;
    }

    public void ButtonPressedd(Button theButton)
    {
        pressed.normalColor = buttonPressed;
        theButton.colors = pressed;
    }

    public void NotPressed()
    {
        stage = 0;
        ButtonPress();
    }
    public void CreateRoom()
    {
        stage = 1;
        ButtonPress();
    }

    public void PrivateRoom()
    {
        stage = 2;
        ButtonPress();
    }
    public void PublicRoom()
    {
        stage = 3;
        ButtonPress();
    }
}
