using System;
using KBCore.Refs;
using UnityEngine;

namespace Assets.Scripts.Runtime.Entities.Player
{
    public class PlayerCameraController : MonoBehaviour
    {
        [SerializeField, Anywhere] InputReader input;
        [SerializeField, Anywhere] GameObject target;
        [SerializeField, Range(0f, 90f)] float topClamp = 45f;
        [SerializeField, Range(0f, -90f)] float bottomClamp = -45f;
        [SerializeField, Range(0f, 90f)] float rightClamp = 45f;
        [SerializeField, Range(0f, -90f)] float leftClamp = -45f;
        [SerializeField, Range(0f, 5f)] float verticalSpeedRotation = 1f;
        [SerializeField, Range(0f, 5f)] float horizontalSpeedRotation = 1f;
        [SerializeField, Range(0f, 5f)] float recenterSpeed = 3f;
        bool isHoldKey, isCurrentDeviceMouse;
        float _cinemachineTargetPitch, _cinemachineTargetYaw;
        Vector2 lookInput;
        Quaternion _targetRotation;

        void Start()
        {
            _targetRotation = target.transform.localRotation;
            var currentEulerAngles = _targetRotation.eulerAngles;
            _cinemachineTargetPitch = currentEulerAngles.x;
            _cinemachineTargetYaw = currentEulerAngles.y;
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

        void OnEnableControlCamera(bool isHolding) => isHoldKey = isHolding;

        void OnLook(Vector2 look, bool device)
        {
            lookInput = look;
            isCurrentDeviceMouse = device;
        }

        void LateUpdate()
        {
            MovementRotation();
            HandleRecenter();
        }

        void MovementRotation()
        {
            if (!isHoldKey) return;

            if (lookInput.sqrMagnitude < 0.01f) return;

            var deltaTimeMultiplier = isCurrentDeviceMouse ? 1f : Time.deltaTime;

            _cinemachineTargetPitch += lookInput.y * verticalSpeedRotation * deltaTimeMultiplier;
            _cinemachineTargetYaw += lookInput.x * horizontalSpeedRotation * deltaTimeMultiplier;

            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, bottomClamp, topClamp);
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, leftClamp, rightClamp);

            target.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0f);
        }

        static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        void HandleRecenter()
        {
            if (isHoldKey) return;

            target.transform.localRotation = Quaternion.Slerp(
                target.transform.localRotation, _targetRotation, recenterSpeed * Time.deltaTime);

            var currentEulerAngles = target.transform.localRotation.eulerAngles;
            _cinemachineTargetPitch = currentEulerAngles.x;
            _cinemachineTargetYaw = currentEulerAngles.y;
        }
    }
}