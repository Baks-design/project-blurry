using System;
using Assets.Scripts.Runtime.Systems.EventBus;
using Assets.Scripts.Runtime.Systems.EventBus.Events;
using Assets.Scripts.Runtime.Utilities.Helpers;
using KBCore.Refs;
using UnityEngine;

namespace Assets.Scripts.Runtime.Entities.Player
{
    public class PlayerCameraController : MonoBehaviour
    {
        [Header("Input Settings")]
        [SerializeField, Anywhere] InputReader input;
        [SerializeField, Anywhere] GameObject target;

        [Header("Rotation Clamps")]
        [SerializeField, Range(0f, 90f)] float topClamp = 45f;
        [SerializeField, Range(0f, -90f)] float bottomClamp = -45f;
        [SerializeField, Range(0f, 90f)] float rightClamp = 45f;
        [SerializeField, Range(0f, -90f)] float leftClamp = -45f;

        [Header("Rotation Speeds")]
        [SerializeField, Range(0f, 5f)] float verticalSpeedRotation = 1f;
        [SerializeField, Range(0f, 5f)] float horizontalSpeedRotation = 1f;
        [SerializeField, Range(0f, 5f)] float recenterSpeed = 3f;
        
        bool _isCurrentDeviceMouse;
        float _cinemachineTargetPitch;
        float _cinemachineTargetYaw;
        Vector2 _lookInput;
        Quaternion _targetRotation;

        public bool IsEnabledCamera { get; private set; }

        EventBinding<CameraMovementEvent> cameraMovementEventBinding;

        void OnEnable()
        {
            input.Look += OnLook;
            input.LookMode += OnEnableControlCamera;
            cameraMovementEventBinding = new EventBinding<CameraMovementEvent>(HandleCameraMovementEvent);
            EventBus<CameraMovementEvent>.Register(cameraMovementEventBinding);
        }

        void OnDisable()
        {
            input.Look -= OnLook;
            input.LookMode -= OnEnableControlCamera;
            EventBus<CameraMovementEvent>.Deregister(cameraMovementEventBinding);
        }

        void OnEnableControlCamera() => IsEnabledCamera = !IsEnabledCamera;

        void OnLook(Vector2 look, bool isDeviceMouse)
        {
            _lookInput = look;
            _isCurrentDeviceMouse = isDeviceMouse;
        }

        void HandleCameraMovementEvent(CameraMovementEvent cameraEvent) => IsEnabledCamera = cameraEvent.isLookValid;

        void Start()
        {
            IsEnabledCamera = false;
            _isCurrentDeviceMouse = false;
            _targetRotation = target.transform.localRotation;
            _cinemachineTargetPitch = _targetRotation.eulerAngles.x;
            _cinemachineTargetYaw = _targetRotation.eulerAngles.y;
        }

        void LateUpdate()
        {
            MovementRotation();
            HandleRecenter();
        }

        void MovementRotation()
        {
            if (!IsEnabledCamera || _lookInput.sqrMagnitude < 0.01f) return;

            var deltaTimeMultiplier = _isCurrentDeviceMouse ? 1f : Time.deltaTime;

            _cinemachineTargetPitch += _lookInput.y * verticalSpeedRotation * deltaTimeMultiplier;
            _cinemachineTargetYaw += _lookInput.x * horizontalSpeedRotation * deltaTimeMultiplier;

            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, bottomClamp, topClamp);
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, leftClamp, rightClamp);

            target.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0f);
        }

        public static float ClampAngle(float angle, float min, float max)
        {
            angle %= 360;
            if ((angle >= -360f) && (angle <= 360f))
            {
                if (angle < -360f)
                    angle += 360f;
                if (angle > 360f)
                    angle -= 360f;
            }
            return Mathf.Clamp(angle, min, max);
        }

        void HandleRecenter()
        {
            if (IsEnabledCamera) return;

            target.transform.localRotation = Quaternion.Slerp(
                target.transform.localRotation, _targetRotation, recenterSpeed * Time.deltaTime);

            var currentEulerAngles = target.transform.localRotation.eulerAngles;
            _cinemachineTargetPitch = currentEulerAngles.x;
            _cinemachineTargetYaw = currentEulerAngles.y;
        }
    }
}