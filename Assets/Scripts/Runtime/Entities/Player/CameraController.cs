using System;
using KBCore.Refs;
using UnityEngine;

namespace Assets.Scripts.Runtime.Entities.Player
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField, Child] Transform tr;
        [SerializeField, Anywhere] InputReader input;
        [SerializeField, Range(0f, 90f)] float upperHorizontalLimit = 35f;
        [SerializeField, Range(0f, 90f)] float lowerHorizontalLimit = 35f;
        [SerializeField, Range(0f, 90f)] float upperVerticalLimit = 35f;
        [SerializeField, Range(0f, 90f)] float lowerVerticalLimit = 35f;
        [SerializeField, Range(1f, 50f)] float cameraSmoothingFactor = 25f;
        [SerializeField] float cameraSpeed = 50f;
        bool isHoldKey;
        float currentXAngle, currentYAngle;
        Vector2 lookInput;

        void Start()
        {
            currentXAngle = tr.localRotation.eulerAngles.x;
            currentYAngle = tr.localRotation.eulerAngles.y;
        }

        void OnEnable()
        {
            input.HoldLook += OnEnableControlCamera;
            input.Look += OnLook;
        }

        void OnDisable()
        {
            input.HoldLook -= OnEnableControlCamera;
            input.Look -= OnLook;
        }

        void OnEnableControlCamera(bool obj) => isHoldKey = obj;

        void OnLook(Vector2 look) => lookInput = look;

        void Update()
        {
            RotateCamera(lookInput.x, -lookInput.y);
            RecenterCamera();
        }

        void RotateCamera(float horizontalInput, float verticalInput)
        {
            if (!isHoldKey) return;

            horizontalInput = Mathf.Lerp(0f, horizontalInput, Time.deltaTime * cameraSmoothingFactor);
            verticalInput = Mathf.Lerp(0f, verticalInput, Time.deltaTime * cameraSmoothingFactor);

            currentXAngle += verticalInput * cameraSpeed * Time.deltaTime;
            currentYAngle += horizontalInput * cameraSpeed * Time.deltaTime;

            currentXAngle = Mathf.Clamp(currentXAngle, -upperVerticalLimit, lowerVerticalLimit);
            currentYAngle = Mathf.Clamp(currentYAngle, -upperHorizontalLimit, lowerHorizontalLimit);

            tr.localRotation = Quaternion.Euler(currentXAngle, currentYAngle, 0f);
        }

        void RecenterCamera()
        {
            if (isHoldKey && tr.localRotation == Quaternion.identity) return;

            currentXAngle = currentYAngle = 0f;
            tr.localRotation = Quaternion.Euler(currentXAngle, currentYAngle, 0f);
        }
    }
}

//Apply motion