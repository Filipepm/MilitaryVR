using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
Evans,
New lighting system
-Supports patterns.
Use previous hierarchy (same as vls_lightgroup)


    --Pattern Setup:
    --Odd numbers is off time
    --Even numbers is on time
    Example: (- off 1ms, / on 1ms)
    ----/-/-/----/-/-/
    pattern = 4,1,1,1,1,1
    

*/
namespace vls
{
    public class vls_lightgroup_new : MonoBehaviour
    {

        private bool Active = false;
        private bool Rotate = false;


        [Header("Set up light")]
        [SerializeField] //use to make a private variable accessable in the editor.
        private light_types type = light_types.led;
        [SerializeField]
        private float InitialDelay = 0;
        public int current_pattern { get; private set; }

        [SerializeField]
        private int[] patternA;
        [SerializeField]
        private int[] patternB;
        [SerializeField]
        private int[] patternC;
        [SerializeField]
        private int[] patternD;
        [SerializeField]
        private int[] patternE;

        
        [Header("Rotator Setup")]
        [SerializeField]
        private GameObject[] Lights;
        [SerializeField]
        private GameObject PhysicalLight;
        [SerializeField]
        private float RotateSpeed = 1; //default to 1 becuase 0 may cause issues?
        [SerializeField]
        private int RotateDirection = 1;
        [SerializeField]
        private float SemiTimeToRotate = 100;

        [Header("Halogen Setup")]
        [SerializeField]
        private float MaxEmission = 0.642f;
        [SerializeField]
        private float MaxSpotIntensity = 8.0f;
        private float spotIntensity = 0;
        [SerializeField]
        private Color EmissiveColor = Color.white;
        private int fade = 0;
        private float current_emission = 0;

        [SerializeField]
        private float fadeMultiplier = 0.5f;
        [SerializeField]
        private float spotFadeMultiplier = 5f;
        private int dir = 0;
        private bool setActive = false;


        void IncrementPattern()
        {
            current_pattern++;
            if (GetCurrentPattern() == null)
            {
                current_pattern = 0;
            }
        }
        public void Activate()
        {
            Active = true;
            StartCoroutine(RunLights());
        }
        public void DeActivate()
        {
            Active = false;
            Rotate = false;
        }
        IEnumerator RunLights()
        {
            yield return new WaitForSeconds(InitialDelay/1000); //do the initial wait.
            if (type == light_types.rotator)
            {
                LightsOn();
                Rotate = true;
                yield return WaitForever();
            }
            else if (type == light_types.semi_rotator)
            {
                LightsOn();
                Rotate = true;
                while (Active)
                {
                    RotateDirection = 1;
                    vls_wait(SemiTimeToRotate);
                    RotateDirection = -1;
                    vls_wait(SemiTimeToRotate);
                }
            }
            else //its LED or Halogen
            {
                while (Active)
                    yield return RunPattern();
            }
            Rotate = false;
            LightsOff();
        }
        IEnumerator RunPattern()
        {
            int[] ActivePattern = GetCurrentPattern();
            for (int i = 0; i < ActivePattern.Length && Active; i++)
            {
                LightsOff();
                yield return vls_wait(ActivePattern[i]);
                if (!Active)
                    break;
                i++;
                LightsOn();
                yield return vls_wait(ActivePattern[i]);
            }
        }

        int[] GetCurrentPattern()
        {
            switch (current_pattern)
            {
                case 1:
                    return patternB;
                case 2:
                    return patternC;
                case 3:
                    return patternD;
                case 4:
                    return patternE;
                case 0:
                default: //unless case 0 is most common, leave here.
                    return patternA;
            }
        }
        void LightsOn()
        {//** do fade if halogen
            if (type == light_types.halogen)
            {
                fade = 1;
            }
            else
            {
                foreach (GameObject light in Lights)
                {
                    light.SetActive(true);
                }
            }
        }
        void LightsOff()
        {//** do fade if halogen
            if(type == light_types.halogen)
            {
                fade = -1;
            }
            else
            {
                foreach (GameObject light in Lights)
                {
                    if (light == PhysicalLight)
                        continue;
                    light.SetActive(false);
                }
            }

        }





        void AssignLights()
        {
            //Iterate through all of the children and add them to the lights array
            Lights = new GameObject[transform.childCount]; //set up the array
            for (int i = 0; i < transform.childCount; i++)      //iterate the list
                Lights[i] = transform.GetChild(i).gameObject;
        }
        // Use this for initialization
        void Start()
        {
            current_pattern = 0;
            AssignLights();
            if (type == light_types.rotator || type == light_types.semi_rotator)
            {
                if (PhysicalLight == null)
                    Debug.LogError("No physical light attached to rotator / semirotator.");
            }
            LightsOff();
        }

        // Update is called once per frame
        void Update()
        {
            if (Rotate)
            {
                if (type == light_types.rotator || type == light_types.semi_rotator) //switch rotatedirection to negative if going backwards.
                {
                    foreach (GameObject light in Lights)
                        light.transform.RotateAround(PhysicalLight.transform.position, PhysicalLight.transform.up, (RotateDirection) * RotateSpeed * Time.deltaTime);
                }
            }



            if (fade != 0) 
            { //do the fade action
              //calculate the current fade amount
              // **

                if (fade == 1)
                { //fade on

                    if (current_emission < MaxEmission)
                        current_emission += Time.deltaTime * fadeMultiplier / 2;
                    if (spotIntensity < MaxSpotIntensity)
                        spotIntensity += Time.deltaTime * spotFadeMultiplier / 2;
                    dir = 1;
                    setActive = true;
                }
                else if (fade == -1 && current_emission > 0)
                { //fade off    
                    if(current_emission > 0)
                        current_emission -= Time.deltaTime * fadeMultiplier;
                    if (spotIntensity > 0)
                        spotIntensity -= Time.deltaTime * spotFadeMultiplier;
                    dir = -1;
                }
                else
                {
                    if (dir == -1)
                        setActive = false;
                    fade = 0;
                }
                if(current_emission != MaxEmission || current_emission != 0)
                {
                    //do the fade
                    foreach (GameObject light in Lights)
                    {
                        Renderer rend = light.GetComponent<Renderer>();
                        Light lite = light.GetComponent<Light>();
                        if(lite != null)
                            lite.intensity = spotIntensity;
                        if(rend != null)
                        {
                            rend.material.shader = Shader.Find("Sonic Ether/Emissive/Textured");
                            rend.material.SetFloat("_EmissionGain", current_emission);
                        }
                        light.SetActive(setActive);
                    }
                }
            }
        }
        IEnumerator WaitForever()
        {
            while (Active)
            {
                yield return vls_wait(3); //wait for 5ms (previously 10, but changed to 3) **this can be changed in order to increase performance.
            }
        }
        IEnumerator vls_wait(float milliseconds) //**!! this method /could/ be resource intensive. If it is, switch the lines directly below the calls to this for the default unity timing.
        {

            //***************************************************************************
            //this is used in case the user deactivates the system during the flash delay.
            //***************************************************************************

            float WaitTime = milliseconds / 1000;//previosuly we had a seconds to milliseconds conversion, we won't have that anymore as we will always use ms.
                                                 /* was previously using waitForSeconds, but this was unreliable for what we're doing here (if user deactivated during the cycle, the cycle would finish first).
                                                 Then I put waitforseconds into a for loop... it was waiting for x seconds, but we also have some time to iterate the loop. So things were unpredictable (eg: 250ms turned into 300ms+ easily)*/

                    //we're gonna wait 1ms each time... and iterate to next loop. When we reach the goal, break from loop.
                    //float StartTime = Time.time; //*****debug
                    float EndTime = Time.time + WaitTime;
            while (Time.time < EndTime)
            {
                yield return new WaitForSeconds(0.001f); //1ms
                if (!Active)
                    break;
            }
            //Debug.Log("Expected:" + WaitTime.ToString() + " Elapsed:"+ (Time.time - StartTime).ToString()); //*****debug
        }
    }

}

