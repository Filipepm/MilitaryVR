using BNG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOnUI : MonoBehaviour
{
    [SerializeField] GameObject UICanvas;
    [SerializeField] GameObject UICanvas2;
    [SerializeField] float coolDown = 0.3f;
    private bool isCoolDown = false;
    private bool canvasOn = false;
    public void Update()
    {
        if (InputBridge.Instance.YButton)
        {
            if (isCoolDown == false)
            {
                UICanvas2.SetActive(true);
                UIPanelToggle();
                StartCoroutine(CoolDown());
            }
        }
    }

    IEnumerator CoolDown()
    {
        isCoolDown = true;
        yield return new WaitForSeconds(coolDown);
        isCoolDown = false;
    }

    private void UIPanelToggle()
    {
        if (canvasOn == false)
        {
            canvasOn = true;
            UICanvas.SetActive(canvasOn);
        }
        else if (canvasOn == true)
        {
            canvasOn = false;
            UICanvas.SetActive(canvasOn);
        }
    }
}
