using UnityEngine;
using System.Collections;

namespace vls_v2
{
    public class VLS_V2_LightController_Rotator : MonoBehaviour
    {
        [SerializeField]
        VLSLightType Type = VLSLightType.rotator;
        [SerializeField]
        private GameObject PivotPoint;
        [SerializeField]
        private GameObject LightsParent;
        [SerializeField]
        private int RotateDirection = 1;
        [SerializeField]
        private float RotateSpeed = 10;
        [SerializeField]
        private static float SemiDegreesToRotate = 90;

        private bool Rotate = false;
        private long Count = 0;
        private bool CoRoutineRunning = false;
        private float x = 360 - (SemiDegreesToRotate / 2);
        private float y = SemiDegreesToRotate / 2;
        int originalDirection = 0;
        float originalDegrees;
        [SerializeField]
        bool returnToNormal = false;
        // Use this for initialization
        Vector3 l_pos;
        void Awake()
        {


            if (!(Type == VLSLightType.rotator || Type == VLSLightType.semi_rotator))
                Debug.LogError("VLS Error - Light type on rotator script is not a rotator type light. Choose either Rotator or SemiRotator.");
            for (int i = 0; i < transform.childCount; i++)      //iterate the list
            {
                if (transform.GetChild(i).name == "Pivot" || 
                    transform.GetChild(i).name == "pivot")
                    PivotPoint = transform.GetChild(i).gameObject;
                for (int a = 0; a < transform.GetChild(i).childCount; a++)
                {
                    if (transform.GetChild(i).GetChild(a).name == "lights" ||
                        transform.GetChild(i).GetChild(a).name == "Lights" ||
                        transform.GetChild(i).GetChild(a).name == "light" ||
                        transform.GetChild(i).GetChild(a).name == "Light")
                        LightsParent = transform.GetChild(i).GetChild(a).gameObject;
                }
            }
            originalDirection = RotateDirection;
            originalDegrees = PivotPoint.transform.localEulerAngles.y;

            l_pos = transform.localPosition;
        }
        public void SwitchToRotator()
        {
            Type = VLSLightType.rotator;
            RotateDirection = originalDirection;
        }
        public void SwitchToSemi()
        {
            Type = VLSLightType.semi_rotator;
        }
        public void Heartbeat()
        {
            if (!Rotate)
                LightsOn(); //turn the lights on if they were freshly activated in the scene - or for some reason were OFF when a heartbeat was sent.
        }
        public void LightsOn()
        {
            Rotate = true;
            LightsParent.SetActive(true);
        }
        public void LightsOff()
        {
            Rotate = false;
            LightsParent.SetActive(false);
        }
        void Update()
        {
            if (Rotate && gameObject.activeInHierarchy || //normal
                returnToNormal && !Rotate && !(PivotPoint.transform.localEulerAngles.y < originalDegrees + 5 && PivotPoint.transform.localEulerAngles.y > originalDegrees -5) //to return to normal
                )
            {
                if (Type == VLSLightType.semi_rotator)
                {
                    float dir = PivotPoint.transform.localEulerAngles.y;
                    if (dir < x && dir > 180)
                        RotateDirection = 1;
                    else if (dir > y && dir <= 180)
                        RotateDirection = -1;
                }
                Vector3 positionSave = PivotPoint.transform.position;
                Vector3 positionSave2 = PivotPoint.transform.localPosition;
                PivotPoint.transform.RotateAround(PivotPoint.transform.position, PivotPoint.transform.up, (RotateDirection) * RotateSpeed * Time.deltaTime);
                PivotPoint.transform.position = positionSave;
                PivotPoint.transform.localPosition = positionSave2;

            }

           transform.localPosition = l_pos;
        }
    }

}
