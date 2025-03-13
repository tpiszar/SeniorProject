/*
Camera controller
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace kroland.fantasymonsters
{
    public class CameraController : MonoBehaviour
    {
        public float cameraMoveSpeed = 0.01f;
        public float cameraRotateSpeed = 240f;
        public float cameraZoomSpeed = 1.0f;
        public bool autoRotate = false;
        public float autoRotateSpeed = 0.1f;
        public GameObject centerObj;

        // Start is called before the first frame update

        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (autoRotate)
            {
                if (centerObj == null)
                {
                    transform.RotateAround(Vector3.zero, Vector3.up, autoRotateSpeed);
                }
                else
                {
                    transform.RotateAround(centerObj.transform.position, Vector3.up, autoRotateSpeed);
                }
            }

            // Move camera with WASD
            if (Input.GetKey(KeyCode.W))
            {
                transform.position += transform.forward * cameraMoveSpeed;
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.position += transform.forward * -cameraMoveSpeed;
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.position += transform.right * cameraMoveSpeed;
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.position += transform.right * -cameraMoveSpeed;
            }
            if (Input.GetKey(KeyCode.LeftControl))
            {
                transform.position += transform.up * -cameraMoveSpeed;
            }
            if (Input.GetKey(KeyCode.Space))
            {
                transform.position += transform.up * cameraMoveSpeed;
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                autoRotate = !autoRotate;
            }

            // Rotate camera with Right Drag
            if (Input.GetMouseButton(1))
            {
                float mouseInputX = Input.GetAxis("Mouse X");
                float mouseInputY = Input.GetAxis("Mouse Y");
                transform.RotateAround(transform.position, Vector3.up, mouseInputX * Time.deltaTime * cameraRotateSpeed);
                transform.RotateAround(transform.position, transform.right, mouseInputY * Time.deltaTime * -cameraRotateSpeed);
            }

            // Zoom in and out of the camera with the mouse wheel
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            transform.position += transform.forward * scroll * cameraZoomSpeed;

            // Move the camera by holding the wheel
            if (Input.GetMouseButton(2))
            {
                float mouseInputX = Input.GetAxis("Mouse X");
                float mouseInputY = Input.GetAxis("Mouse Y");
                transform.position -= transform.right * mouseInputX * cameraMoveSpeed;
                transform.position -= transform.up * mouseInputY * cameraMoveSpeed;
            }
        }

        public void toggleAutoRotate()
        {
            autoRotate = !autoRotate;
        }
    }
}