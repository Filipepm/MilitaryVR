using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoloVideoScript : MonoBehaviour
{
    private GameObject holoVid1;
    private GameObject holoVid2;
    public GameObject holoVidOG;
    public Transform parentSpawn;


    public HoloVideoObject holo1;
    public HoloVideoObject holo2;
    public int i = 0;

    public Object Intro1;
    public Object Intro2;
    public Object Positive1;
    public Object Positive2;
    public Object Positive3;
    public Object Negative1;
    public Object Negative2;
    public Object Negative3;

    public void FindURL(string url)
    {
        if(url == "intro1")
        {
            string newURL = "Assets/StreamingAssets/Introduction 1.mp4";
            ChangeHolo(newURL);
        }
        if (url == "intro2")
        {
            string newURL = "Assets/StreamingAssets/Introduction 2.mp4";
            ChangeHolo(newURL);
        }
        if (url == "negative1")
        {
            string newURL = "Assets/StreamingAssets/Negative 1.mp4";
            ChangeHolo(newURL);
        }
        if (url == "negative2")
        {
            string newURL = "Assets/StreamingAssets/Negative 2.mp4";
            ChangeHolo(newURL);
        }
        if (url == "negative3")
        {
            string newURL = "Assets/StreamingAssets/Negative 3.mp4";
            ChangeHolo(newURL);
        }
        if (url == "positive1")
        {
            string newURL = "Assets/StreamingAssets/Positive 1.mp4";
            ChangeHolo(newURL);
        }
        if (url == "positive2")
        {
            string newURL = "Assets/StreamingAssets/Positive 2.mp4";
            ChangeHolo(newURL);
        }
        if (url == "positive3")
        {
            string newURL = "Assets/StreamingAssets/Positive 3.mp4";
            ChangeHolo(newURL);
        }
    }

    public void ChangeHolo(string url)
    {
        Debug.Log(url);
        if (i == 0)
        {
            StartCoroutine(Change1(url));
            i = 1;
        }
        else
        {
            StartCoroutine(Change2(url));
            i = 0;
        }
        
    }

    public IEnumerator Change1(string url)
    {
        if(holoVid2 != null)
        {
            holoVid2.GetComponent<HoloVideoObject>().Pause();
        }
        holoVid1 = Instantiate(holoVidOG);
        holoVid1.transform.SetParent(parentSpawn);
        holoVid1.transform.localPosition = Vector3.zero;
        holoVid1.GetComponent<HoloVideoObject>().URLChange(url);
        yield return new WaitForSeconds(.8f);
        holoVid1.GetComponent<HoloVideoObject>().OverridePlay();
        if (holoVid2 != null)
        {
            holoVid2.GetComponent<HoloVideoObject>().AudioToggle();
            Destroy(holoVid2);
        }
        
    }

    public IEnumerator Change2(string url)
    {
        if (holoVid1 != null)
        {
            holoVid1.GetComponent<HoloVideoObject>().Pause();
        }
        holoVid2 = Instantiate(holoVidOG);
        holoVid2.transform.SetParent(parentSpawn);
        holoVid2.transform.localPosition = Vector3.zero;
        holoVid2.GetComponent<HoloVideoObject>().URLChange(url);
        yield return new WaitForSeconds(.8f);
        holoVid2.GetComponent<HoloVideoObject>().OverridePlay();
        if (holoVid1 != null)
        {
            holoVid1.GetComponent<HoloVideoObject>().AudioToggle();
            Destroy(holoVid1);
        }
    }
}
