using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class FFD_SirenControl : MonoBehaviour
{

    // Audio player link
    public AudioSource sirenOutput;

    // Audio Clip Storage
    public AudioClip Horn;

    public AudioClip Man;
    public AudioClip Man_Rumble;
    public AudioClip Wail;
    public AudioClip Wail_Rumble;
    public AudioClip Yelp;
    public AudioClip Yelp_Rumble;
    public AudioClip Prty;
    public AudioClip Prty_Rumble;

    public Vector2 yelpAutoInterval;
    public Vector2 wailAutoInterval;
    public Vector2 prtyAutoInterval;

    // Audio status variables.
    public bool rumbleOn_New = false;
    public bool rumbleOn_Current = false;

    public SirenState SirenState_New = SirenState.Off;

    public SirenState SirenState_Current = SirenState.Off;
    public SirenState SirenState_Previous = SirenState.Off;

    public enum SirenState { Off, Horn, Man, Wail, Yelp, Prty };


    public KeyCode Horn_Key = KeyCode.T;
    public KeyCode Man_Key = KeyCode.Y;

    public KeyCode Wail_Key = KeyCode.B;
    public KeyCode Yelp_Key = KeyCode.N;
    public KeyCode Prty_Key = KeyCode.H;

    public KeyCode Rumble_Key = KeyCode.G;

    public UnityEvent OnSirenActiveUpdate;

    private SirenState lastState;
    private SirenState LastState
    {
        set
        {
            if (lastState != value)
            {
                lastState = value;

                if (OnUpdateSiren != null)
                    OnUpdateSiren.Invoke();
            }
        }
    }

    public event System.Action OnUpdateSiren;
    public bool canOverride = true;
    // Use this for initialization
    void Awake()
    {
        sirenOutput.dopplerLevel = 0.0F;

        rumbleOn_New = rumbleOn_Current = false;
        SirenState_New = SirenState_Current = SirenState_Previous = SirenState.Off;

        LastState = SirenState_Current;

    }


    public SirenState RPCSirenState = SirenState.Off;
    public event System.Action OnRPCSirenChange;

    public bool isAutoSiren = false;

    public void SetAutoSiren(bool state)
    {
        isAutoSiren = state;
    }
    int autoSirenState = -1;
    float lastChange = 0;

    public void Update()
    {


        if (isAutoSiren)
        {
            if (lastChange < Time.time)
            {
                autoSirenState++;
                if (autoSirenState > 2)
                    autoSirenState = 0;

                switch (autoSirenState)
                {
                    case 0: lastChange = Time.time + Random.Range(yelpAutoInterval.x, yelpAutoInterval.y); OnYelpKeyDn(); break;
                    case 1: lastChange = Time.time + Random.Range(wailAutoInterval.x, wailAutoInterval.y); OnWailKeyDn(); break;
                    case 2: lastChange = Time.time + Random.Range(prtyAutoInterval.x, prtyAutoInterval.y); OnPrtyKeyDn(); break;
                }
            }


        }

        if (SirenState_Current == SirenState.Horn || SirenState_Current == SirenState.Prty || SirenState_Current == SirenState.Wail || SirenState_Current == SirenState.Yelp || SirenState_Current == SirenState.Man)
            OnSirenActiveUpdate.Invoke();

        UpdateInput();
        UpdateSiren();
    }


    

    public void OnManualKey()
    {

        if (SirenState_Current == SirenState.Prty || SirenState_Current == SirenState.Wail || SirenState_Current == SirenState.Yelp) {
            return;
        }
        if (SirenState_Current == SirenState.Off)
        {
            SirenState_New = SirenState.Man;
        }
        else if (SirenState_Current == SirenState.Prty && (SirenState_Previous == SirenState.Off || SirenState_Previous == SirenState.Prty))
        {
            SirenState_New = SirenState.Horn;
        }
    }

    public void OnSilence()
    {
        SirenState_New = SirenState.Off;
    }

    public void OnManKeyUp()
    {

        if (SirenState_Current == SirenState.Prty || SirenState_Current == SirenState.Wail || SirenState_Current == SirenState.Yelp) {
            return;
        }
        if (SirenState_Current == SirenState.Yelp)
        {
            SirenState_New = SirenState.Prty;
        }
        else if (SirenState_Current == SirenState.Wail)
        {
            SirenState_New = SirenState.Prty;
        }
        else if (SirenState_Current == SirenState.Prty)
        {
            SirenState_New = SirenState_Previous;
        }
        else if (SirenState_Current == SirenState.Man)
        {
            SirenState_New = SirenState.Off;
        }
        else if (SirenState_Current == SirenState.Horn && SirenState_Previous == SirenState.Prty)
        {
            SirenState_New = SirenState.Prty;
        }
    }

    public void OnRumbleKey()
    {
        rumbleOn_New = !rumbleOn_New;
    }

    public void OnHornKey()
    {
        SirenState_New = SirenState.Horn;
    }

    public void OnHornKeyUp()
    {
        SirenState_New = SirenState_Previous;
    }

    public void OnYelpKeyDn()
    {
        if (AllowSirenChange(SirenState_Current))
           
        {
            if (SirenState_Current != SirenState.Yelp)
            {
                SirenState_New = SirenState.Yelp;
                
            }
            else
            {
                SirenState_New = SirenState.Off;
                
                                                              
            }
        }
    }

    public void OnWailKeyDn()
    {
        if (AllowSirenChange(SirenState_Current))
        {
            if (SirenState_Current != SirenState.Wail)
            {
                SirenState_New = SirenState.Wail;


            }
            else
            {
                SirenState_New = SirenState.Off;






            }
        }
    }

    public void OnPrtyKeyDn()
    {
        if (AllowSirenChange(SirenState_Current))
        {
            if (SirenState_Current != SirenState.Prty)
            {
                SirenState_New = SirenState.Prty;
                

                
                
            }
            else
            {
                SirenState_New = SirenState.Off;
                
                

                
                
            }
        }
    }


    void UpdateInput()
    {

      //  if (SirenState_Current == SirenState.Man && Man_Rumble || SirenState_Current == SirenState.Yelp && Yelp_Rumble || SirenState_Current == SirenState.Prty && Prty_Rumble || SirenState_Current == SirenState.Wail && Wail_Rumble)
       // {
            if (Input.GetKeyDown(Rumble_Key))
            {
                OnRumbleKey();
            }
        //}

        // Horn Control...
        if (Horn != null)
        {
            if (Input.GetKey(Horn_Key) && !Input.GetKey(Man_Key))
            {
                OnHornKey();
            }

            if (Input.GetKeyUp(Horn_Key) && !Input.GetKey(Man_Key))
            {
                OnHornKeyUp();
            }
        }

        if (Input.GetKeyDown(Wail_Key))
        {
            OnWailKeyDn();
        }

        if (Input.GetKeyDown(Yelp_Key))
        {
            OnYelpKeyDn();
         
        }

        if (Input.GetKeyDown(Prty_Key))
        {
            OnPrtyKeyDn();
            
        }

        // Man Control...
        if (Input.GetKeyDown(Man_Key)) {
            OnManualKey();
        }


        if (Input.GetKeyUp(Man_Key)) {
            OnManKeyUp();
        }

    }

    bool AllowSirenChange(SirenState _state)
    {
        if (_state == SirenState.Off)
        {
            return true;
        }

        if (_state == SirenState.Wail)
        {
            return true;
        }
        if (_state == SirenState.Yelp)
        {
            return true;
        }
        if (_state == SirenState.Prty)
        {
            return true;
        }
        return false;
    }

    public void UpdateSiren()
    {
        if (SirenState_New == SirenState_Current && rumbleOn_New == rumbleOn_Current)
        {
            return;
        }

        SetSiren(SirenState_New, rumbleOn_New);
        LastState = SirenState_New;

        // Keep track of where we were before...
        if (SirenState_Current == SirenState.Off || SirenState_Current == SirenState.Prty || SirenState_Current == SirenState.Wail || SirenState_Current == SirenState.Yelp)
        {
            SirenState_Previous = SirenState_Current;
        }
        // reset SirenStates for new settings...
        rumbleOn_Current = rumbleOn_New;
        SirenState_Current = SirenState_New;
    }

    /// <summary>
    /// This script does nothing but set the sound to the one requested, with a runbleOn mod bool.
    /// </summary>
    /// <param name="_SirenState">Siren state.</param>
    /// <param name="_rumbleOn">If set to <c>true</c> rumble on.</param>
    public void SetSiren(SirenState _SirenState, bool _rumbleOn)
    {
        if (_SirenState == SirenState.Off)
        {
            sirenOutput.Stop();
            sirenOutput.clip = null;
            return;
        }


        if (_SirenState == SirenState.Horn)
        {
            if (Horn) {
                sirenOutput.Stop();
                sirenOutput.clip = Horn;
                sirenOutput.Play();
            }

            return;
        }

        if (_SirenState == SirenState.Man)
        {
            float PlaybackTime = 0f;
            if (sirenOutput.clip == Man || sirenOutput.clip == Man_Rumble)
            {
                PlaybackTime = sirenOutput.time;
            }

           

            sirenOutput.Stop();
            if (!_rumbleOn)
            {
                sirenOutput.clip = Man;
            }
            else
            {
                sirenOutput.clip = Man_Rumble;
            }
            //sirenOutput.time = Mathf.Clamp(PlaybackTime, 0, sirenOutput.clip.length - 0.01f);

            sirenOutput.Play();
            return;
        }

        if (_SirenState == SirenState.Wail)
        {
           

            float PlaybackTime = 0f;
            if (sirenOutput.clip == Wail || sirenOutput.clip == Wail_Rumble)
            {
                PlaybackTime = sirenOutput.time;
            }

            sirenOutput.Stop();
            if (!_rumbleOn)
            {
                sirenOutput.clip = Wail;
            }
            else
            {
                sirenOutput.clip = Wail_Rumble;
            }
            sirenOutput.time = Mathf.Clamp(PlaybackTime, 0, sirenOutput.clip.length);
            sirenOutput.time = Mathf.Clamp(PlaybackTime, 0, sirenOutput.clip.length - 0.01f);



            sirenOutput.Play();
            return;
        }

        if (_SirenState == SirenState.Yelp)
        {
            float PlaybackTime = 0f;
            if (sirenOutput.clip == Yelp || sirenOutput.clip == Yelp_Rumble)
            {
                PlaybackTime = sirenOutput.time;
            }

            sirenOutput.Stop();
            if (!_rumbleOn)
            {
                sirenOutput.clip = Yelp;
            }
            else
                sirenOutput.clip = Yelp_Rumble;

            // Temporarily disabled
            //sirenOutput.time = Mathf.Clamp(PlaybackTime, 0, sirenOutput.clip.length - 0.01f);
            sirenOutput.Play();
            return;
        }

        if (_SirenState == SirenState.Prty)
        {
            float PlaybackTime = 0f;
            if (sirenOutput.clip == Prty || sirenOutput.clip == Prty_Rumble)
            {
                PlaybackTime = sirenOutput.time;
            }

            sirenOutput.Stop();
            if (!_rumbleOn)
            {
                sirenOutput.clip = Prty;
            }
            else
            {
                sirenOutput.clip = Prty_Rumble;
            }
            sirenOutput.time = Mathf.Clamp(PlaybackTime, 0, sirenOutput.clip.length - 0.01f);
            sirenOutput.Play();
            return;
        }
    }
}