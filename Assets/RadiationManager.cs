using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadiationManager : MonoBehaviour
{
    public GameObject completeUI;
    public int taskCompletion = 0;

    public void TaskManager()
    {
        taskCompletion++;
        if(taskCompletion == 10)
        {
            StartCoroutine(EndTask());
        }
    }

    public IEnumerator EndTask()
    {
        completeUI.SetActive(true);

        yield return new WaitForSeconds(5f);

        completeUI.SetActive(false);
    }
}
