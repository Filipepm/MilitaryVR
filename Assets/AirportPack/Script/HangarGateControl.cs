using UnityEngine;

namespace AirportPack
{
    public class HangarGateControl : MonoBehaviour

    {
        Animator m_Animator;
        [Range(0.0f, 1f)]
        public float gateDoorControl;

        void Start()
        {
            m_Animator = gameObject.GetComponent<Animator>();
        }

        void Update()
        {
            m_Animator.Play("gateDoorControl", 0, gateDoorControl);
        }
    }
}