using UnityEngine;

namespace AirportPack
{
    public class AircraftControl : MonoBehaviour

    {
        Animator m_Animator;
        [Range(0.0f, 1f)]
        public float gears;
        [Range(0.0f, 1f)]
        public float speedBrakes;
        [Range(0.0f, 1f)]
        public float flaps;
        [Range(0.0f, 1f)]
        public float thrustReverser;
        [Range(0.0f, 1f)]
        public float doors;
        [Range(0.0f, 1f)]
        public float ailerons;

        void Start()
        {
            m_Animator = gameObject.GetComponent<Animator>();
        }

        void Update()
        {
            m_Animator.Play("gears", 0, gears);
            m_Animator.Play("speedBrakes", 1, speedBrakes);
            m_Animator.Play("flaps", 2, flaps);
            m_Animator.Play("thrustReverser", 3, thrustReverser);
            m_Animator.Play("doors", 4, doors);
            m_Animator.Play("ailerons", 5, ailerons);
        }
    }
}
