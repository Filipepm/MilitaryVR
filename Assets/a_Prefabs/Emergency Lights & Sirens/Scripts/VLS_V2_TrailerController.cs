using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace vls_v2
{
    public class VLS_V2_TrailerController:MonoBehaviour
    {

        /*
        Evans, 5/29/2016
        Vehicle emergency lighting system trailer controller


        */
        /***********************************
        Other variables, misc.
        ***********************************/
        System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();

        /***********************************
        Individual Lights
        ***********************************/
        private VLSLightObject[] LightObjects;



        /***********************************
        Misc.
        ***********************************/
        public int MaxModes = 10;
        public int CurrentMode = 0;//default of 0 (off)

        [SerializeField]
        bool overridesExistingLightsWhenActive = false;
        [SerializeField]
        bool startDisabled = false;
        [SerializeField]
        GameObject[] ExistingLightsToOverride;
        [SerializeField]
        List<Renderer> ExistingRenderers = new List<Renderer>();
        [SerializeField]
        List<Light> ExistingSpotLights = new List<Light>();

       
        void Awake()
        {
            //Add the children objects to the list.
            //Iterate through all of the children and add them to the lightobject array
            LightObjects = new VLSLightObject[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                LightObjects[i] = new VLSLightObject(
                            transform.GetChild(i).GetComponent<VLS_V2_LightController_Flasher>(),
                            transform.GetChild(i).GetComponent<VLS_V2_LightController_Rotator>(),
                            transform.GetChild(i).GetComponent<VLS_V2_LightController_ColouredRotator>(),
                            transform.GetChild(i).GetComponent<VLS_V2_VisionController>()
                           );
            }
            //determine the max modes
            for (int i = 0; i < LightObjects.Length; i++)
            {
                if (MaxModes > LightObjects[i].GetMaxPatterns())
                    MaxModes = LightObjects[i].GetMaxPatterns();
            }


            if (overridesExistingLightsWhenActive && ExistingLightsToOverride.Length > 0)
            {//this controller will override existing lights when active (turn them off).
                for (int i = 0; i < ExistingLightsToOverride.Length; i++)
                {
                    if (ExistingLightsToOverride[i] == null) continue;

                    ExistingRenderers.AddRange(ExistingLightsToOverride[i].GetComponentsInChildren<Renderer>());
                    ExistingSpotLights.AddRange(ExistingLightsToOverride[i].GetComponentsInChildren<Light>());
                }
            }
        }


        void Start()
        {
            foreach (VLSLightObject obj in LightObjects)
                obj.LightsOff(); //turn off all of the lights.
            if(startDisabled)
            {
                
            }
        }

        public void TurnAllOn()
        {
           // netHelper.RaiseEvent("turnallon");
            p_TurnAllOn();
        }
        private void p_TurnAllOn()
        {
            areAllOff = false;
            for (int i = 0; i < ExistingRenderers.Count; i++)
                ExistingRenderers[i].enabled = false;
            for (int i = 0; i < ExistingSpotLights.Count; i++)
                ExistingSpotLights[i].enabled = false;
            CurrentMode = 1;
            StartCoroutine(Run());
        }

        public void TurnAllOff()
        {
          //  netHelper.RaiseEvent("turnalloff");
            p_TurnAllOff();
        }

        public bool areAllOff = true;
		public int cacheMode;
        private void p_TurnAllOff()
        {
			cacheMode = CurrentMode;
            areAllOff = true;
             for (int i = 0; i < ExistingRenderers.Count; i++)
                ExistingRenderers[i].enabled = true;
            for (int i = 0; i < ExistingSpotLights.Count; i++)
                ExistingSpotLights[i].enabled = true;
            CurrentMode = 0;
            timer.Reset();
        }

        bool startup = false;

       
        public void IncreaseMode(int i)
        {
          /*  NetBaseHelper.NetBuffer buffer = new NetBaseHelper.NetBuffer(null);
            buffer.UShort(2);
            buffer.Int(i);
            buffer.Flush();
            netHelper.RaiseEvent(buffer);*/
            p_IncreasedMode(i -1);
        }
        private void p_IncreasedMode(int i)
        {
            if (CurrentMode == 0)
                startup = true;
            CurrentMode = i ;
            IncreaseMode();
        }

        //onincreasesimple
        public void IncreaseMode()
        {
            p_IncreasedMode();
        }


        private void p_IncreasedMode()
        {
            if (CurrentMode == 0 || startup)
            { //turning the system on
                startup = false;
                CurrentMode++;
                //start the coroutuine
                StartCoroutine(Run());
            }
            else
            { //changing the pattern or turning the system off
                CurrentMode++;
                if (CurrentMode > MaxModes)
                {
                    CurrentMode = 0;
                    timer.Reset();
                }
            }
        }

        IEnumerator Run()
        {
            for (int i = 0; i < LightObjects.Length; i++)
            {
                LightObjects[i].Start();
            }
            long TimeElapsed = 1; //time in ms
            timer.Start();
            while (CurrentMode != 0)
            {
                for (int i = 0; i < LightObjects.Length; i++)
                {
                    if (CurrentMode == 0)
                        break;
                    LightObjects[i].Tick(CurrentMode, TimeElapsed);
                    //yield return null;
                }
                timer.Stop();
                TimeElapsed = timer.ElapsedMilliseconds;
                timer.Reset();
                timer.Start();
                yield return new WaitForSeconds(0.001f);
            }
            for (int i = 0; i < LightObjects.Length; i++)
            {
                LightObjects[i].End();
            }
            yield return null;
        }

    }

}
