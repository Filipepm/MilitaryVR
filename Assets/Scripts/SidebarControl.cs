using BNG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidebarControl : MonoBehaviour
{
    [SerializeField] public List<GameObject> dModel = new List<GameObject>();
    [SerializeField] public List<GameObject> videos = new List<GameObject>();
    [SerializeField] public List<GameObject> photos = new List<GameObject>();
    [SerializeField] public List<GameObject> alphabetList = new List<GameObject>();
    [SerializeField] public List<GameObject> recentList = new List<GameObject>(50);
    public void UIFilter(int count)
    {
        switch (count)
        {
            //3D Models
            case 0:
                AllModels(false);
                DModel();
                break;

            //Videos
            case 1:
                AllModels(false);
                Videos();
                break;

            //Photos
            case 2:
                AllModels(false);
                Photos();
                break;
        }
    }

    private void DModel()
    {
        for (int i = 0; i < dModel.Count; i++)
        {
            dModel[i].SetActive(true);
        }
    }
    private void Videos()
    {
        for (int i = 0; i < videos.Count; i++)
        {
            videos[i].SetActive(true);
        }
    }
    private void Photos()
    {
        for (int i = 0; i < photos.Count; i++)
        {
            photos[i].SetActive(true);
        }
    }
    private void AllModels(bool type)
    {
        for (int i = 0; i < photos.Count; i++)
        {
            photos[i].SetActive(type);
        }

        for (int i = 0; i < videos.Count; i++)
        {
            videos[i].SetActive(type);
        }

        for (int i = 0; i < dModel.Count; i++)
        {
            dModel[i].SetActive(type);
        }
    }

    public void AlphabeticalOrder()
    {
        AllModels(true);
        for(int i = 0; i < alphabetList.Count; i++)
        {
            Debug.Log(i);
            alphabetList[i].transform.SetSiblingIndex(i);
        }
    }
    
    public void StackOrder(GameObject module)
    {
        recentList.Remove(module);
        recentList.Insert(0, module);
    }
    
    public void RecentOrder()
    {
        AllModels(false);
        int i = 0;
        foreach(var stackItem in recentList)
        {
            stackItem.SetActive(true);
            alphabetList[i].transform.SetSiblingIndex(i);
            i++;
        }
    }
}
