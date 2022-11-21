using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptEnabler : MonoBehaviour
{
    public MonoBehaviour baseBehaviour;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate ()
    {
        if (baseBehaviour.enabled == false)
            baseBehaviour.enabled = true;
	}
}
