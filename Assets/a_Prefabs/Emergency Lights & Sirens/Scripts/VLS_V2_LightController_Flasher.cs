using UnityEngine;
using System.Collections;

namespace vls_v2
{
    public class VLS_V2_LightController_Flasher : MonoBehaviour
    {
        /*Evans, VLS V2 flasher controller*/

        public VLSLightType Type = VLSLightType.led;

        /***********************************
        Other variables, misc.
        ***********************************/
            private long Count = 0;
            private int CurrentPosition = 0;
            private int CurrentPattern = 0;
            private bool CurrentStatus = false;
            private GameObject[] Lights;

        /***********************************
        Patterns
        ***********************************/
        [Header("Pattern Setup")]
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
            

        /***********************************
        Fading
        ***********************************/
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
            private bool lightsCompletelyOff = true;
        /***
         Steady Burn
         ***/
        [SerializeField]
            private bool IsSteadyBurn = false;

        public void Awake()
        {
            if (!(Type == VLSLightType.led || Type == VLSLightType.halogen))
                Debug.LogError("VLS ERROR - Light type on flasher script is not a flasher type light. Choose either LED or Halogen.");
            //*********** ADD ALL LIGHTS TO LIST ************
            //Iterate through all of the children and add them to the lights array
            Lights = new GameObject[transform.childCount]; //set up the array
            for (int i = 0; i < transform.childCount; i++)      //iterate the list
                Lights[i] = transform.GetChild(i).gameObject;
        }
        public void Update()
        {
            //*********** Do the fade for halogen type lights ************
            if (fade != 0)
            { //do the fade action

                //calculate the current fade amount
                if (fade == 1)
                { //fade on

                    if (current_emission < MaxEmission)
                        current_emission += Time.deltaTime * fadeMultiplier;
                    if (spotIntensity < MaxSpotIntensity)
                        spotIntensity += Time.deltaTime * spotFadeMultiplier;
                    dir = 1;
                    setActive = true;
                }
                else if (fade == -1)
                { //fade off    
                    if (current_emission > 0)
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

                //do the actual fade
                if (current_emission != MaxEmission   || current_emission != 0 &&
                    spotIntensity != MaxSpotIntensity || spotIntensity != 0)
                {
                    //do the fade
                    foreach (GameObject light in Lights)
                    {
                        Renderer rend = light.GetComponent<Renderer>();
                        Light lite = light.GetComponent<Light>();
                        if (lite != null)
                            lite.intensity = spotIntensity;
                        if (rend != null)
                        {
                            rend.material.shader = Shader.Find("Sonic Ether/Emissive/Textured");
                            rend.material.SetFloat("_EmissionGain", current_emission);
                        }
			if (current_emission <= 0)
				light.SetActive(false);
                        if (lightsCompletelyOff)
                        {
                            setActive = false;
                        }
                        light.SetActive(setActive);
                    }
                }
            }
            //*********** end fade code ************
        }
        public void Tick(int PassedPattern, long TimeElapsed)
        {//this is called from the VLS_V2VehicleController script
            if (CurrentPattern != PassedPattern)
            {
                CurrentPattern = PassedPattern;
                Count = 0;
                CurrentPosition = 0;
                LightsOff();
            }
            if (IsSteadyBurn && PassedPattern != 0)
            {
                LightsOn();
            } else if(IsSteadyBurn && PassedPattern == 0)
            {
                LightsOff();
            }
            else
            {
                Count += TimeElapsed;
                if (CurrentPosition > GetCurrentPattern().Length)
                    Debug.LogError("VLS ERROR - Pattern length too short. Pattern#:" + CurrentPattern.ToString() + ", Object:" + gameObject.name + ", Position:" + CurrentPosition.ToString());
                if (GetCurrentPattern().Length > 0)
                {
                    while (Count >= GetCurrentPattern()[CurrentPosition])
                    {
                        Count = Count - GetCurrentPattern()[CurrentPosition]; //update count
                        CurrentPosition++;                                    //update position
                        if (CurrentPosition >= GetCurrentPattern().Length)
                        {
                            CurrentPosition = 1;
                        }
                        ToggleLights();
                    }
                }
            }
            if (CurrentPattern != PassedPattern)
            {
                CurrentPattern = PassedPattern;
                Count = 0;
                CurrentPosition = 0;
                LightsOff();
            }
            Count+=TimeElapsed;
            if (CurrentPosition > GetCurrentPattern().Length)
                Debug.LogError("VLS ERROR - Pattern length too short. Pattern#:" + CurrentPattern.ToString() + ", Object:" + gameObject.name + ", Position:" + CurrentPosition.ToString());
            if(GetCurrentPattern().Length > 0)
            {
                while (Count >= GetCurrentPattern()[CurrentPosition])
                {
                    Count = Count - GetCurrentPattern()[CurrentPosition]; //update count
                    CurrentPosition++;                                    //update position
                    if (CurrentPosition >= GetCurrentPattern().Length)
                    {
                        CurrentPosition = 1;
                    }
                    ToggleLights();
                }
            }


        }
        public void ToggleLights()
        {
            if (CurrentStatus)
                LightsOff();
            else
                LightsOn();
        }
        void LightsOn()
        {//** do fade if halogen
            CurrentStatus = true;
            if (Type == VLSLightType.halogen)
                fade = 1;
            else
            {
                foreach (GameObject light in Lights)
                    light.SetActive(true);

            }
            lightsCompletelyOff = false;
        }
        public void LightsOffEnd()
        {
            CurrentPosition = 0;
            Count = 0;
            CurrentPattern = 0;
            LightsOff();
            lightsCompletelyOff = true;
            
        }
        public void KillNow()
        { //support for turntable scene.
            current_emission = 0;
            spotIntensity = 0;
            LightsOffEnd();
        }
        private void LightsOff()
        {//** do fade if halogen
            if (gameObject.GetComponent<VLS_V2_VisionController>() != null)
                return; //right now, this will override some of the other lights in the vision pods.
            CurrentStatus = false;
            if (Type == VLSLightType.halogen)
            {
                fade = -1;
                
                
            }
            else
            {
                foreach (GameObject light in Lights)
                    light.SetActive(false);
            }

        }
        public int GetMaxPattern()
        {
            if (patternA.Length > 0 &&
                patternB.Length > 0 &&
                patternC.Length > 0 &&
                patternD.Length > 0 &&
                patternE.Length > 0)
                return 5;
            if (patternA.Length > 0 &&
                patternB.Length > 0 &&
                patternC.Length > 0 &&
                patternD.Length > 0)
                return 4;
            if (patternA.Length > 0 &&
                patternB.Length > 0 &&
                patternC.Length > 0)
                return 3;
            if (patternA.Length > 0 &&
                patternB.Length > 0)
                return 2;
            if (patternA.Length > 0)
                return 1;
            return 0;
        }
        int[] GetCurrentPattern()
        {
            switch (CurrentPattern)
            {
                case 2:
                    return patternB;
                case 3:
                    return patternC;
                case 4:
                    return patternD;
                case 5:
                    return patternE;
                case 1:
                default: //unless case 0 is most common, leave here.
                    return patternA;
            }
        }




    }

}