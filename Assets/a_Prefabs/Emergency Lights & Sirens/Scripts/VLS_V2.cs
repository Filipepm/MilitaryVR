using UnityEngine;
using System.Collections.Generic;


namespace vls_v2
{
    public struct VLSLightObject
    {
        VLS_V2_LightController_Flasher flasherScript;
        VLS_V2_LightController_Rotator rotatorScript;
        VLS_V2_LightController_ColouredRotator colouredRotatorScript;
        VLS_V2_VisionController visionScript;
        public VLSLightObject(VLS_V2_LightController_Flasher flasher, VLS_V2_LightController_Rotator rotator, VLS_V2_LightController_ColouredRotator _colouredRotatorScript, VLS_V2_VisionController vision)
        {
            flasherScript = flasher;
            rotatorScript = rotator;
            colouredRotatorScript = _colouredRotatorScript;
            visionScript = vision;
            if(visionScript != null)
            {
                if (flasherScript != null)
                    flasherScript.enabled = false;
                if (rotatorScript != null)
                    rotatorScript.enabled = false;
                if (colouredRotatorScript != null)
                    colouredRotatorScript.enabled = false;
            }
        }
        public void Tick(int currentPattern, long Count)
        {
            if (flasherScript != null && flasherScript.gameObject.activeInHierarchy)
                flasherScript.Tick(currentPattern, Count); //we need to pass this so that the script knows which pattern to run.
            if (rotatorScript != null && rotatorScript.gameObject.activeInHierarchy)
                rotatorScript.Heartbeat();
            if (colouredRotatorScript != null && colouredRotatorScript.gameObject.activeInHierarchy)
                colouredRotatorScript.Heartbeat();
        }
        public void ChangePattern(int Pattern)
        {//mainly used for the visionSLR lightbar
            if(visionScript != null)
            {
                switch (visionScript.GetCurrentType(Pattern))
                {
                    case VLS_V2_VisionController.PodType.colored:
                        if (flasherScript != null)
                            flasherScript.enabled = false;
                        if (rotatorScript != null)
                            rotatorScript.enabled = false;
                        if (colouredRotatorScript != null)
                            colouredRotatorScript.enabled = true;
                        break;
                    case VLS_V2_VisionController.PodType.flasher:
                        if (flasherScript != null)
                            flasherScript.enabled = true;
                        if (rotatorScript != null)
                            rotatorScript.enabled = false;
                        if (colouredRotatorScript != null)
                            colouredRotatorScript.enabled = false;
                        break;
                    case VLS_V2_VisionController.PodType.semi:
                        if (flasherScript != null)
                            flasherScript.enabled = false;
                        if (rotatorScript != null)
                        {
                            rotatorScript.enabled = true;
                            rotatorScript.SwitchToSemi();
                        }
                        if (colouredRotatorScript != null)
                            colouredRotatorScript.enabled = false;
                        break;
                    case VLS_V2_VisionController.PodType.rotator:
                        if (flasherScript != null)
                            flasherScript.enabled = false;
                        if (rotatorScript != null)
                        {
                            rotatorScript.enabled = true;
                            rotatorScript.SwitchToRotator();
                        }
                        if (colouredRotatorScript != null)
                            colouredRotatorScript.enabled = false;
                        break;
                    default:
                    case VLS_V2_VisionController.PodType.off:
                        if (flasherScript != null)
                        {
                            flasherScript.LightsOffEnd();
                            flasherScript.enabled = false;
                        }
                        if (rotatorScript != null)
                        {
                            rotatorScript.LightsOff();
                            rotatorScript.enabled = false;
                        }
                        if (colouredRotatorScript != null)
                        {
                            colouredRotatorScript.LightsOff();
                            colouredRotatorScript.enabled = false;
                        }
                        break;
                }
            }
        }
        public int GetMaxPatterns()
        {
            if (visionScript != null && visionScript.gameObject.activeInHierarchy)
                return visionScript.GetMaxPatterns();
            else if (flasherScript != null && flasherScript.gameObject.activeInHierarchy)
                return flasherScript.GetMaxPattern();
            else if (rotatorScript != null && rotatorScript.gameObject.activeInHierarchy)
                return 1; //**update this
            else
                return 10;
        }
        public void Start()
        {
            if (rotatorScript != null && rotatorScript.gameObject.activeInHierarchy)
                rotatorScript.LightsOn();
            if (colouredRotatorScript != null && colouredRotatorScript.gameObject.activeInHierarchy)
                colouredRotatorScript.LightsOn();
        }
        public void End()
        {
            LightsOff();
        }
        public void LightsOff()
        {
            if (flasherScript != null && flasherScript.gameObject.activeInHierarchy)
                flasherScript.LightsOffEnd();
            else if (rotatorScript != null && rotatorScript.gameObject.activeInHierarchy)
                rotatorScript.LightsOff();
            if (colouredRotatorScript != null && colouredRotatorScript.gameObject.activeInHierarchy)
                colouredRotatorScript.LightsOff();

        }
        public void KillNow()
        { //support for the turntable scene.
            if (flasherScript != null && flasherScript.gameObject.activeInHierarchy)
                flasherScript.KillNow();
            else if (rotatorScript != null && rotatorScript.gameObject.activeInHierarchy)
                rotatorScript.LightsOff();
            else if (colouredRotatorScript != null && colouredRotatorScript.gameObject.activeInHierarchy)
                colouredRotatorScript.LightsOff();

        }
    }

    public enum VLSLightType
    {
        led, halogen, rotator, semi_rotator
    }
}