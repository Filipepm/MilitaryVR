using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; // Required when Using UI elements.


namespace vls_v2
{
    public class VLS_V2_VehicleController : MonoBehaviour
    {
        /*
        Evans, 5/20/2016 - Updated 7/7/2016
        Vehicle emergency lighting system controller version 2.1

        This script gets added into the main VLS gameobject, use the following hierarchy:

        Items in () are scripts attached to that specific object
        Items in "" are spellings of the actual gameobject (very important)


        EmergencyLights         (VLS_V2_VehicleController)
        |--LED type lights      (VLS_V2_LightController_Flasher)
        | |-IndividualLight
        | |-IndividualLight
        |--Halogen Type Lights  (VLS_V2_LightController_Flasher)
        | |-IndividualLight     
        |--Rotator Type Light   (VLS_V2_LightController_Rotator)
        | |-PivotObject "Pivot"
        | |-pivotObjects (shell of light, etc)
        | |-pivotObjects
        | |-LightParent "Lights"
        | | |-Emissive
        | | |-Spotlight

        **set up directionals and takedowns like trailer**


        *Note:
        1. Do not use timings under ~30-35ms, they will not be represented properly. This needs more testing.

        */
        /***********************************
        Other variables, misc.
        ***********************************/
        public KeyCode LightsKey = KeyCode.J;
        public KeyCode TakeDownKey = KeyCode.L;
        public KeyCode DirectionalKey = KeyCode.K;

        System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();

        /***********************************
        Individual Lights
        ***********************************/
        private VLSLightObject[] LightObjects = new VLSLightObject[0];



        /***********************************
        Misc.
        ***********************************/
        public int MaxModes = 10;
        public int CurrentMode = 0;//default of 0 (off)
        [SerializeField]
        GameObject TakeDownParent;
        [SerializeField]
        GameObject DirectionalParent;
        public VLS_V2_TrailerController TakeDownController;
        public VLS_V2_TrailerController DirectionalController;

        [SerializeField]
        List<GameObject> Trailers = new List<GameObject>();
        AudioSource BeepSource;

        public bool takedownStatus = false;
        public Slider LightbarSlider;

        int DirectionalPattern = 0;
        // Use this for initialization
        void Awake()
        {
            //add the car script to this.
            Transform t = transform;
            
           
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
            BeepSource = gameObject.GetComponent<AudioSource>();
        
            for (int i = 0; i < LightObjects.Length; i++)
            {
                if (MaxModes > LightObjects[i].GetMaxPatterns())
                    MaxModes = LightObjects[i].GetMaxPatterns();
            }
            if (TakeDownParent != null && TakeDownParent.GetComponent<VLS_V2_TrailerController>() != null)
                TakeDownController = TakeDownParent.GetComponent<VLS_V2_TrailerController>();
            else
                TakeDownController = null;
            if (DirectionalParent != null && DirectionalParent.GetComponent<VLS_V2_TrailerController>() != null)
                DirectionalController = DirectionalParent.GetComponent<VLS_V2_TrailerController>();
            else
                DirectionalController = null;
        }

        void Start()
        {
            if (LightbarSlider != null)
                LightbarSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
            else
                Debug.LogWarning("No lightbar slider attached to lightbar controller");
			

            foreach (VLSLightObject obj in LightObjects)
                obj.LightsOff(); //turn off all of the lights.

            StartCoroutine(Run());

        }
        bool executedAction = false;

		public bool valueCheckOnEnable = false;
		public void OnEnable()
		{
			if (valueCheckOnEnable == true)
				ValueChangeCheck ();
		}

        public void ValueChangeCheck()
        {
            SetMainPattern((int)LightbarSlider.value);
        }

        void Update()
        {
            //foreach (VLSLightObject obj in LightObjects)
             //   obj.LightsOff();
            {

                executedAction = true;
                if (Input.GetKeyUp(LightsKey))
                {
                    if (OnIncreasedMode != null)
                        OnIncreasedMode(CurrentMode);

                    Debug.Log("Increased mode");
                    IncreaseMode();
                }
                if (Input.GetKeyUp(TakeDownKey))
                    ToggleTakeDowns();
                if (Input.GetKeyUp(DirectionalKey))
                {
                    if(OnIncreasedDirectional != null && DirectionalController != null)
                    {
                        OnIncreasedDirectional(DirectionalController.CurrentMode);
                    }
                    IncreaseDirectional();
                }
            }

        }
        public void DirectionalLeft()
        {
           

            if (DirectionalPattern != 1)
                SetDirectionalPattern(1);
            else
                DirectionalOff();
        }
        public void DirectionalRight()
        {
           
            if (DirectionalPattern != 3)
                SetDirectionalPattern(3);
            else
                DirectionalOff();
        }
        public void DirectionalCenter()
        {
  
            if (DirectionalPattern != 2)
                SetDirectionalPattern(2);
            else
                DirectionalOff();
        }
        public void DirectionalOff()
        {
           
            SetDirectionalPattern(0);
        }
        public void SetDirectionalPattern(int i)
        {
            DirectionalPattern = i;
            if (DirectionalController == null)
                return;
            BeepSource.Stop();
            BeepSource.Play();
            DirectionalController.IncreaseMode(i);
        }
        public void SetMainPattern(int i)
        {
            CurrentMode = i - 1;
            IncreaseMode();
        }

        public event IncreasedModeCallback OnToggleTakedowns;
        public void ToggleTakeDowns()
        {
           
            if (TakeDownController == null)
                return;

            if (OnToggleTakedowns != null)
            {
                if (takedownStatus == true)
                    OnToggleTakedowns(0);
                else
                    OnToggleTakedowns(1);
            }

            BeepSource.Stop();
            BeepSource.Play();
            if (takedownStatus)
            {
                takedownStatus = false;
                //turn off the takedowns
                TakeDownController.TurnAllOff();
            }
            else
            {
                takedownStatus = true;
                //turn on the takedowns
                TakeDownController.TurnAllOn();
            }
        }

        public event IncreasedModeCallback OnIncreasedDirectional;
        public void IncreaseDirectional()
        {
            if (DirectionalController == null)
                return;
            
            BeepSource.Stop();
            BeepSource.Play();
            DirectionalController.IncreaseMode();
        }

        public delegate void IncreasedModeCallback(int currentMode);
        public event IncreasedModeCallback OnIncreasedMode;
        public void IncreaseMode()
        { //increase the mode, or turn the system off.
            BeepSource.Stop();
            BeepSource.Play();
            for (int i = 0; i < Trailers.Count; i++)
            {
                VLS_V2_TrailerController controller = Trailers[i].GetComponent<VLS_V2_TrailerController>();
                if (controller != null)
                {
                    Trailers[i].GetComponent<VLS_V2_TrailerController>().CurrentMode = CurrentMode;
                    Trailers[i].GetComponent<VLS_V2_TrailerController>().IncreaseMode();
                }
                else
                    Debug.LogError("Trailer does not have trailer lights script attached but is assigned in editor.");
            }
            if (CurrentMode == 0)
            { //turning the system on
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
            for (int i = 0; i < LightObjects.Length; i++)
            {
                LightObjects[i].ChangePattern(CurrentMode);
            }
            //LightbarSlider.value = CurrentMode;
        }

        public void IncreasedModeDisable()
        {
          
            for (int i = 0; i < Trailers.Count; i++)
            {
                VLS_V2_TrailerController controller = Trailers[i].GetComponent<VLS_V2_TrailerController>();
                /*if (controller != null)
                    Trailers[i].GetComponent<VLS_V2_TrailerController>().IncreaseMode();
                else
                    Debug.LogError("Trailer does not have trailer lights script attached but is assigned in editor.");  */
            }
            if (CurrentMode == 0)
            { //turning the system on
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
            for (int i = 0; i < LightObjects.Length; i++)
            {
                LightObjects[i].ChangePattern(CurrentMode);
            }
        }

		[HideInInspector]
		public int cacheMode ;
        public void KillNow()
        { //support for the turntable scene - probably wont be used ever in the actual game.
			cacheMode = CurrentMode;
			CurrentMode = 0;
            timer.Reset();
            for (int i = 0; i < LightObjects.Length; i++)
            {
                LightObjects[i].KillNow();
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
                yield return null;
            }
            for (int i = 0; i < LightObjects.Length; i++)
            {
                LightObjects[i].End();
            }
            yield return null;
        }
    }

}


