using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
Evans - April 2016
Lighting control script.

Usage:
    Add this script to a lightgroup parent object, define type and delays.
    Use with vehicle_light_system.cs

    Use with following hierarchy:
    EmergencyLightParent (vehicle_light_system.cs)
    -OrginizationParent
    --LightGroup (vls_lightgroup.cs)<<
    ---EmissiveLight
    ---SpotLight
    
    You can add as many "EmissiveLight', 'SpotLight', 'OrginizationParents' as desired.
    Additional lightgroups that are not within this hierarchy can be added via the 'vehicle_light_system.cs' script.

*/
namespace vls
{
    public class vls_lightgroup : MonoBehaviour
    {

        [SerializeField]
        private float delay; //delay in milliseconds before the light will start flashing
        [SerializeField]
        private float on_time; //total time light is on during cycle (blank/zero/whatever for rotator)
        [SerializeField]
        private float off_time; //total time light is off during cycle (blank/zero/whatever for rotator)
        [SerializeField]
        private float rotator_speed = 750f; //the speed the light rotates (set a default, because I think zero will crash the editor)
        [SerializeField]
        private light_types type; //what type of light (see enum light_types)
        [SerializeField]
        private GameObject physical_light; //the physical light that will rotate in it's center.
        private GameObject[] lights; //the array of lights (emissive and spots)

        private bool active = false; //set this to true to start the light flashing, set to false to stop the light
        private bool previously_active = false; //this is set if the code is running, set to false if the code is stopped.
        private bool doRotate = false; //we're running this through the update method, so we need a marker.
        [SerializeField]
        private bool doSemiRotate = false;
        [SerializeField]
        private bool semi_rotate_right = false;



        // Use this for initialization
        void Start()
        {
            //Iterate through all of the children and add them to the lights array
            lights = new GameObject[transform.childCount]; //set up the array
            for (int i = 0; i < transform.childCount; i++)      //iterate the list
                lights[i] = transform.GetChild(i).gameObject;
            light_off();
        }

        // Update is called once per frame
        void Update()
        {
            if (active && !previously_active)
                StartCoroutine(start_sequence());
            if (doRotate)
                rotate_light();
            if (doSemiRotate)
                semi_rotate_light();
        }
        public void activate()
        {
            active = true;
        }
        public void deactivate()
        {
            active = false;
            previously_active = false;
        }
        IEnumerator start_sequence()
        {
            previously_active = true;
            yield return vls_wait(delay); //wait for the delay to finish
            //yield return new WaitForSeconds(delay / 1000);
            switch (type)
            {
                case light_types.led://run the LED light
                    while (active)
                    {
                        //run the lights.
                        light_on();
                        yield return vls_wait(on_time);
                        //yield return new WaitForSeconds(flash_length / 1000);//time to wait for the light being on
                        light_off();
                        yield return vls_wait(off_time);
                        //yield return new WaitForSeconds(flash_frequency / 1000);//time to wait for the light being off
                    }
                    light_off();
                    break;
                case light_types.rotator:
                    light_on();
                    doRotate = true;
                    while (active)
                        yield return vls_wait(0.1f);
                    doRotate = false;
                    light_off();
                    break;
                case light_types.semi_rotator://run the rotator light
                    light_on();
                    doSemiRotate = true;
                    while (active)
                    {
                        semi_rotate_right = false;
                        yield return vls_wait(on_time); //wait while the light turns to the right
                        semi_rotate_right = true;
                        yield return vls_wait(on_time);//wait while the light turns to the left.
                    }
                    doSemiRotate = false;
                    light_off();
                    break;
            }
        }
        private void light_on() //turn the light on!
        {
            switch (type)
            {
                case light_types.semi_rotator:
                case light_types.led:
                case light_types.rotator: //eventually, I'd like to move the rotator to a fade-in, similar to a halogen light (which I'd also like to add).
                    foreach (GameObject light in lights)
                        light.SetActive(true);
                    break;
            }
        }
        private void light_off() //turn the light off!
        {
            switch (type)
            {
                case light_types.semi_rotator:
                case light_types.led:
                case light_types.rotator:
                    foreach (GameObject light in lights)
                    {
                        if (light == physical_light)
                            continue;
                        light.SetActive(false);
                    }
                    break;
            }
        }

        private void rotate_light()
        {
            //rotate the light (for rotators, of course)
            if (lights.Length < 1)
            {
                Debug.LogError("VLS warning - lights less than 1 for this object");
            }
            else
            {
                foreach (GameObject light in lights)
                    light.transform.RotateAround(physical_light.transform.position, physical_light.transform.up, rotator_speed * Time.deltaTime);
            }
        }
        private void semi_rotate_light()
        {
            //rotate the light (for rotators, of course)
            if (lights.Length < 1)
            {
                Debug.LogError("VLS warning - lights less than 1 for this object");
            }
            else if(semi_rotate_right) //rotate the light to the right
            {
                foreach (GameObject light in lights)
                    light.transform.RotateAround(physical_light.transform.position, physical_light.transform.up, rotator_speed * Time.deltaTime); //rotate the light to the right
            }
            else if(!semi_rotate_right) //rotate the light to the left
            {
                foreach (GameObject light in lights)
                    light.transform.RotateAround(physical_light.transform.position, physical_light.transform.up, -(rotator_speed) * Time.deltaTime); //rotate the light to the left... this will work because it works with the rotators above.
            }
        }








        private IEnumerator vls_wait(float seconds) //**!! this method /could/ be resource intensive. If it is, switch the lines directly below the calls to this for the default unity timing.
        {

            //***************************************************************************
            //this is used in case the user deactivates the system during the flash delay.
            //***************************************************************************

            float WaitTime = seconds / 1000;
            /* was previously using waitForSeconds, but this was unreliable for what we're doing here (if user deactivated during the cycle, the cycle would finish first).
            Then I put waitforseconds into a for loop... it was waiting for x seconds, but we also have some time to iterate the loop. So things were unpredictable (eg: 250ms turned into 300ms+ easily)*/

            //we're gonna wait 1ms each time... and iterate to next loop. When we reach the goal, break from loop.
            //float StartTime = Time.time; //*****debug
            float EndTime = Time.time + WaitTime;
            while (Time.time < EndTime)
            {
                yield return new WaitForSeconds(0.001f); //1ms
            }
            //Debug.Log("Expected:" + WaitTime.ToString() + " Elapsed:"+ (Time.time - StartTime).ToString()); //*****debug
        }
    }


}
