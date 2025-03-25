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
        bool _isHoldKey;
        bool _isCurrentDeviceMouse;
        float _cinemachineTargetPitch;
        float _cinemachineTargetYaw;
        Vector2 _lookInput;
        Quaternion _targetRotation;

        void OnEnable()
        {
            input.LookMode += OnEnableControlCamera;
            input.Look += OnLook;
        }

        void OnDisable()
        {
            input.LookMode -= OnEnableControlCamera;
            input.Look -= OnLook;
        }

        void OnEnableControlCamera()
        {
            _isHoldKey = !_isHoldKey;
            EventBus<LockPlayerMovementEvent>.Raise(new LockPlayerMovementEvent { isMoveValid = !_isHoldKey });
        }

        void OnLook(Vector2 look, bool isDeviceMouse)
        {
            _lookInput = look;
            _isCurrentDeviceMouse = isDeviceMouse;
        }

        void Start()
        {
            _isHoldKey = false;
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
            if (!_isHoldKey || _lookInput.sqrMagnitude < 0.01f) return;

            var deltaTimeMultiplier = _isCurrentDeviceMouse ? 1f : Time.deltaTime;

            _cinemachineTargetPitch += _lookInput.y * verticalSpeedRotation * deltaTimeMultiplier;
            _cinemachineTargetYaw += _lookInput.x * horizontalSpeedRotation * deltaTimeMultiplier;

            _cinemachineTargetPitch = Mathf.Clamp(_cinemachineTargetPitch, bottomClamp, topClamp);
            _cinemachineTargetYaw = Mathf.Clamp(_cinemachineTargetYaw, leftClamp, rightClamp);

            target.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0f);
        }

        void HandleRecenter()
        {
            if (_isHoldKey) return;

            target.transform.localRotation = Quaternion.Slerp(
                target.transform.localRotation, _targetRotation, recenterSpeed * Time.deltaTime);

            var currentEulerAngles = target.transform.localRotation.eulerAngles;
            _cinemachineTargetPitch = currentEulerAngles.x;
            _cinemachineTargetYaw = currentEulerAngles.y;
        }
    }
}