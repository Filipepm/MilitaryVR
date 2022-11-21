using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIControl : MonoBehaviour
{
    public GameObject Main;
    public GameObject Library;
    public GameObject Connect;
    public void Start()
    {        
        Main.SetActive(true);
        Library.SetActive(false);
        Connect.SetActive(false);        
    }
    public void MainUI()
    {
        Main.SetActive(true);
        Library.SetActive(false);
        Connect.SetActive(false);
    }
    public void LibraryUI()
    {
        Main.SetActive(false);
        Library.SetActive(true);
        Connect.SetActive(false);
    }
    public void ConnectUI()
    {
        Main.SetActive(false);
        Library.SetActive(false);
        Connect.SetActive(true);
    }
}
