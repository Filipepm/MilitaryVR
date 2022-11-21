using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
Evans - April 2016
Lighting system script.

Usage:
    Add this script to a lighting system parent object, define any additional lightgroups not within hierarchy.
    Use with vls_lightgroup.cs

    Use with following hierarchy:
    EmergencyLightParent (vehicle_light_system.cs)<<
    -OrginizationParent
    --LightGroup (vls_lightgroup.cs)
    ---EmissiveLight
    ---SpotLight
    
    You can add as many "EmissiveLight', 'SpotLight', 'OrginizationParents' as desired.
    Additional lightgroups that are not within this hierarchy can be added via the 'vehicle_light_system.cs' script.
    You can define the key on this script too. (eg: L), press this key with CONTROL in order to turn off.

*/
namespace vls
{
    public class vehicle_light_system : MonoBehaviour
    {

        public KeyCode turnOn = KeyCode.L; //debug use control + this key to turn off

        [SerializeField]
        private GameObject[] light_groups;

        [SerializeField]
        private List<GameObject> LightGroups_not_within_children = new List<GameObject>();

        // Use this for initialization
        void Start()
        {
            List<GameObject> light_groups_temp = new List<GameObject>(); //this is temporary, lists can be slow esp. for what we're doing with this one.
            //add all the custom lightgroups
            foreach (GameObject lightgroup in LightGroups_not_within_children)
            {
                light_groups_temp.Add(lightgroup);
            }

            //Iterate through all of the children and add them to the light_groups array
            GameObject[] temp_group = new GameObject[transform.childCount]; //set up the array
            for (int i = 0; i < transform.childCount; i++)              //iterate the list
                temp_group[i] = transform.GetChild(i).gameObject;       //add all of the children to the temp group (this includes the rotator groups and led groups)


            foreach (GameObject group in temp_group)
            {
                vls_lightgroup script = group.GetComponent<vls_lightgroup>();
                vls_lightgroup_new script_new = group.GetComponent<vls_lightgroup_new>();
                if (script == null && script_new == null)
                {
                    for (int i = 0; i < group.transform.childCount; i++)                //iterate the list
                        light_groups_temp.Add(group.transform.GetChild(i).gameObject); //add all of the children to the temp group (this includes the rotator groups and led groups)
                }
                else
                {
                    light_groups_temp.Add(group);
                }
            }
            light_groups = light_groups_temp.ToArray(); //put into a wonderful array :)
        }

        public bool running = false;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(turnOn) && !running)
                activate();
            if (Input.GetKey(turnOn) && Input.GetKey(KeyCode.LeftControl) && running)
                deactivate();
        }
        [SerializeField]
        public void activate()
        {
            running = true;
            foreach (GameObject lightgroup in light_groups)
            {
                vls_lightgroup script = lightgroup.GetComponent<vls_lightgroup>();
                vls_lightgroup_new script_new = lightgroup.GetComponent<vls_lightgroup_new>();
                if (script != null)
                    script.activate();
                else if (script_new != null)
                    script_new.Activate();
                else if (script == null && script_new == null)
                    Debug.Log("Error - You have not attached lighting scripts to your lihgts!");
            }
        }
        public void deactivate()
        {
            running = false;
            foreach (GameObject lightgroup in light_groups)
            {
                vls_lightgroup script = lightgroup.GetComponent<vls_lightgroup>();
                vls_lightgroup_new script_new = lightgroup.GetComponent<vls_lightgroup_new>();
                if (script != null)
                    script.deactivate();
                else if (script_new != null)
                    script_new.DeActivate();
                else if (script == null && script_new == null)
                    Debug.Log("Error - You have not attached lighting scripts to your lihgts!");
            }
        }



    }
    public enum light_types
    {
        led, rotator, semi_rotator, halogen
    }
}

