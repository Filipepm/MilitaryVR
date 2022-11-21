using UnityEngine;
using System.Collections;

namespace vls_v2
{
	//Script Created in parody of the other LightController scripts, using a similar programming style.
    public class VLS_V2_LightController_ColouredRotator : MonoBehaviour
    {
        [SerializeField]
        VLSLightType Type = VLSLightType.rotator;
        [SerializeField]
        private GameObject PivotPoint;
        [SerializeField]
		private GameObject LightsAParent;
		[SerializeField]
		private GameObject LightsBParent;
        [SerializeField]
        private int RotateDirection = 1;
        [SerializeField]
        private float RotateSpeed = 10;
        [SerializeField]
		private static float SemiDegreesToRotate = 90;
		[SerializeField]
		private Vector2 LightsAActivationRange = new Vector2(180f, 360f);

        private bool Rotate = false;
        private long Count = 0;
        private bool CoRoutineRunning = false;
        private float x = 360 - (SemiDegreesToRotate / 2);
        private float y = SemiDegreesToRotate / 2;
        // Use this for initialization
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
					if (transform.GetChild(i).GetChild(a).name == "lights A" ||
						transform.GetChild(i).GetChild(a).name == "lights a" ||
						transform.GetChild(i).GetChild(a).name == "Lights A" ||
						transform.GetChild(i).GetChild(a).name == "Lights a" ||
						transform.GetChild(i).GetChild(a).name == "light A" ||
						transform.GetChild(i).GetChild(a).name == "light a" ||
						transform.GetChild(i).GetChild(a).name == "Light A" ||
						transform.GetChild(i).GetChild(a).name == "Light a")
                        LightsAParent = transform.GetChild(i).GetChild(a).gameObject;
					else if (transform.GetChild(i).GetChild(a).name == "lights B" ||
						transform.GetChild(i).GetChild(a).name == "lights b" ||
						transform.GetChild(i).GetChild(a).name == "Lights B" ||
						transform.GetChild(i).GetChild(a).name == "Lights b" ||
						transform.GetChild(i).GetChild(a).name == "light B" ||
						transform.GetChild(i).GetChild(a).name == "light b" ||
						transform.GetChild(i).GetChild(a).name == "Light B" ||
						transform.GetChild(i).GetChild(a).name == "Light b")
						LightsBParent = transform.GetChild(i).GetChild(a).gameObject;
                }
            }
			LightsOff ();
        }
        public void Heartbeat()
        {
            if (!Rotate)
                LightsOn(); //turn the lights on if they were freshly activated in the scene - or for some reason were OFF when a heartbeat was sent.
        }
        public void LightsOn()
        {
            Rotate = true;
			CalculateLightingState ();
        }
        public void LightsOff()
        {
			Rotate = false;
			//Turns off both sets of lights
			LightsAParent.SetActive(false);
            LightsBParent.SetActive(false);
			// - Anthony
        }
		//Used to work out which lights should be on based around the current Y rotation
		private void CalculateLightingState () {
			if (PivotPoint.transform.localEulerAngles.y < LightsAActivationRange.y &&
			    PivotPoint.transform.localEulerAngles.y > LightsAActivationRange.x) {
				LightsAParent.SetActive (true);
				LightsBParent.SetActive (false);
			} else {
				LightsAParent.SetActive (false);
				LightsBParent.SetActive (true);
			}

		}
		// - Anthony
		void OnDrawGizmos () {
			Gizmos.color = Color.red;
			float moveDirection = LightsAActivationRange.x + 90;
			float xMove = Mathf.Cos (Mathf.Deg2Rad * moveDirection);
			float yMove = Mathf.Sin (Mathf.Deg2Rad * moveDirection);
			Vector3 thatWay = new Vector3 (yMove, 0, xMove);
			Gizmos.DrawLine (transform.position, transform.position + thatWay);

			moveDirection = LightsAActivationRange.y + 90;
			xMove = Mathf.Cos (Mathf.Deg2Rad * moveDirection);
			yMove = Mathf.Sin (Mathf.Deg2Rad * moveDirection);
			thatWay = new Vector3 (yMove, 0, xMove);
			Gizmos.DrawLine (transform.position, transform.position + thatWay);
		}
        void Update() {
            if (Rotate && gameObject.activeInHierarchy) {
                if (Type == VLSLightType.semi_rotator) {
                    float dir = PivotPoint.transform.localEulerAngles.y;
                    if (dir < x && dir > 180)
                        RotateDirection = 1;
                    else if (dir > y && dir <= 180)
                        RotateDirection = -1;
                }
                PivotPoint.transform.RotateAround(PivotPoint.transform.position, PivotPoint.transform.up, (RotateDirection) * RotateSpeed * Time.deltaTime);
				CalculateLightingState ();
			}
        }
    }
}