using UnityEngine;
using System;
using System.Reflection;

namespace AirportPack
{
    public class CameraControl : MonoBehaviour
    {

        public float Speed = 10f;
        public int MouseSensitivity = 100;

        void Start()
        {

        }

        void Update()
        {

            if (Input.GetKey(KeyCode.KeypadMultiply))
            {
                Vector3 center = transform.position + transform.forward * 20;
                transform.RotateAround(center, Vector3.up, Time.deltaTime * Speed);
                transform.LookAt(center);
            }
            if (Input.GetKey(KeyCode.KeypadDivide))
            {
                Vector3 center = transform.position + transform.forward * 20;
                transform.RotateAround(center, Vector3.up, 0 - Time.deltaTime * Speed);
                transform.LookAt(center);
            }
            if (Input.GetKey(KeyCode.KeypadPlus))
            {
                transform.position += transform.up * Speed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.KeypadMinus))
            {
                transform.position -= transform.up * Speed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.Minus))
            {
                GetComponent<Camera>().fieldOfView += 1f;
            }
            if (Input.GetKey(KeyCode.Equals))
            {
                GetComponent<Camera>().fieldOfView -= 1f;
            }

            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W) || Input.GetMouseButton(0))
            {
                transform.position += transform.forward * Speed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S) || Input.GetMouseButton(1))
            {
                transform.position -= transform.forward * Speed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                transform.position += transform.right * Speed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                transform.position -= transform.right * Speed * Time.deltaTime;
            }
            transform.localEulerAngles += new Vector3(Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime, Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime, 0);
            Speed += Input.GetAxis("Mouse ScrollWheel") * 10f;
        }
    }
}