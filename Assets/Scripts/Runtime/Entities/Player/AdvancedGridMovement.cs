using System;
using KBCore.Refs;
using UnityEngine;

namespace Assets.Scripts.Runtime.Entities.Player
{
    public class AdvancedGridMovement : MonoBehaviour
    {
        [SerializeField, Self] Transform _transform;
        [Header("Grid Settings")]
        [SerializeField] LayerMask collisionLayerMask;
        [SerializeField, Range(1f, 5f)] float gridSize = 3f;
        [Header("Walk Settings")]
        [SerializeField, Range(1f, 5f)] float walkSpeed = 1f;
        [SerializeField, Range(1f, 10f)] float turnSpeed = 5f;
        [SerializeField]
        AnimationCurve walkSpeedCurve = new(
            new Keyframe(0f, 0f),
            new Keyframe(0.2f, 0.1f),
            new Keyframe(0.5f, 0.7f),
            new Keyframe(1f, 1f)
        );
        [SerializeField]
        AnimationCurve walkHeadBobCurve = new(
            new Keyframe(0f, 0f),
            new Keyframe(0.25f, 1f),
            new Keyframe(0.5f, 0f),
            new Keyframe(0.75f, -1f),
            new Keyframe(1f, 0f)
        );
        [Header("Run Settings")]
        [SerializeField, Range(1f, 5f)] float runningSpeed = 1.5f;
        [SerializeField]
        AnimationCurve runningSpeedCurve = new(
            new Keyframe(0f, 0f),
            new Keyframe(0.1f, 0.3f),
            new Keyframe(0.5f, 0.8f),
            new Keyframe(1f, 1f)
        );
        [SerializeField]
        AnimationCurve runningHeadBobCurve = new(
            new Keyframe(0f, 0f),
            new Keyframe(0.2f, 1f),
            new Keyframe(0.4f, 0f),
            new Keyframe(0.6f, -1f),
            new Keyframe(0.8f, 0f),
            new Keyframe(1f, 1f)
        );
        [Header("Step Settings")]
        [SerializeField, Range(1f, 5f)] float maximumStepHeight = 2f;
        float _rotationTime, _curveTime, _stepTime, _stepTimeCounter, _currentSpeed;
        const float RightHand = 90f, LeftHand = -RightHand, ApproximationThreshold = 0.025f;
        Vector3 _moveFromPosition, _moveTowardsPosition;
        Quaternion _rotateFromDirection, _rotateTowardsDirection;
        AnimationCurve _currentAnimationCurve, _currentHeadBobCurve;
        readonly Collider[] _collidersBuffer = new Collider[10];

        public bool IsStationary => !IsMoving && !IsRotating;
        bool IsMoving => HeightInvariantVector(_transform.position) != HeightInvariantVector(_moveTowardsPosition);
        bool IsRotating => _transform.rotation != _rotateTowardsDirection;
        Vector3 CalculateForwardPosition => _transform.forward * gridSize;
        Vector3 CalculateStrafePosition => _transform.right * gridSize;

        public event Action BlockedEvent = delegate { };
        public event Action StepEvent = delegate { };
        public event Action TurnEvent = delegate { };

        void Start()
        {
            _moveTowardsPosition = _transform.position;
            _rotateTowardsDirection = _transform.rotation;
            _currentAnimationCurve = walkSpeedCurve;
            _currentHeadBobCurve = walkHeadBobCurve;
            _currentSpeed = walkSpeed;
            _stepTime = 1f / gridSize;
        }

        void Update()
        {
            AnimateMovement();
            AnimateRotation();
        }

        public void SwitchToWalking() => UpdateMovementSettings(walkSpeedCurve, walkHeadBobCurve, walkSpeed);

        public void SwitchToRunning() => UpdateMovementSettings(runningSpeedCurve, runningHeadBobCurve, runningSpeed);

        void UpdateMovementSettings(AnimationCurve newCurve, AnimationCurve newHeadBobCurve, float newSpeed)
        {
            var currentPosition = _currentAnimationCurve.Evaluate(_curveTime);
            var newPosition = newCurve.Evaluate(_curveTime);

            if (newPosition < currentPosition)
                _curveTime = FindTimeForValue(currentPosition, newCurve);

            _currentSpeed = newSpeed;
            _currentAnimationCurve = newCurve;
            _currentHeadBobCurve = newHeadBobCurve;
        }

        float FindTimeForValue(float position, AnimationCurve curve)
        {
            var result = 1f;
            while (position < curve.Evaluate(result) && result > 0f)
                result -= ApproximationThreshold;
            return result;
        }

        void AnimateRotation()
        {
            if (!IsRotating) return;

            _rotationTime += Time.deltaTime;
            _transform.rotation = Quaternion.Slerp(_rotateFromDirection, _rotateTowardsDirection, _rotationTime * turnSpeed);
            CompensateRoundingErrors();
        }

        void AnimateMovement()
        {
            if (!IsMoving) return;

            _curveTime += Time.deltaTime * _currentSpeed;
            _stepTimeCounter += Time.deltaTime * _currentSpeed;

            if (_stepTimeCounter > _stepTime)
            {
                _stepTimeCounter = 0f;
                StepEvent.Invoke();
            }

            var currentPositionValue = _currentAnimationCurve.Evaluate(_curveTime);
            var currentHeadBobValue = _currentHeadBobCurve.Evaluate(_curveTime * gridSize);

            var heightVariance = HeightInvariantVector(_moveTowardsPosition) - HeightInvariantVector(_moveFromPosition);
            var targetHeading = Vector3.Normalize(heightVariance);
            var newPosition = _moveFromPosition + targetHeading * (currentPositionValue * gridSize);
            newPosition.y = maximumStepHeight;

            var ray = new Ray(newPosition, -Vector3.up);
            if (Physics.Raycast(ray, out var hit))
                newPosition.y = maximumStepHeight - hit.distance + currentHeadBobValue;
            else
                newPosition.y = currentHeadBobValue;

            _transform.position = newPosition;

            CompensateRoundingErrors();
        }

        void CompensateRoundingErrors()
        {
            if (_transform.rotation == _rotateTowardsDirection)
                _transform.rotation = _rotateTowardsDirection;

            var currentPosition = HeightInvariantVector(_transform.position);
            var target = HeightInvariantVector(_moveTowardsPosition);

            if (currentPosition == target)
            {
                currentPosition = HeightInvariantVector(_moveTowardsPosition);
                currentPosition.y = _transform.position.y;

                _transform.position = currentPosition;
                _curveTime = 0f;
                _stepTimeCounter = 0f;
            }
        }

        Vector3 HeightInvariantVector(Vector3 inVector) => new(inVector.x, 0f, inVector.z);

        public void MoveForward() => CollisionCheckedMovement(CalculateForwardPosition);

        public void MoveBackward() => CollisionCheckedMovement(-CalculateForwardPosition);

        public void StrafeRight() => CollisionCheckedMovement(CalculateStrafePosition);

        public void StrafeLeft() => CollisionCheckedMovement(-CalculateStrafePosition);

        void CollisionCheckedMovement(Vector3 movementDirection)
        {
            if (!IsStationary) return;

            var targetPosition = _moveTowardsPosition + movementDirection;
            if (FreeSpace(targetPosition))
            {
                _moveFromPosition = _transform.position;
                _moveTowardsPosition = targetPosition;
            }
            else
                BlockedEvent.Invoke();
        }

        bool FreeSpace(Vector3 targetPosition)
        {
            var delta = (targetPosition - _moveTowardsPosition) * 0.6f;
            var center = _moveTowardsPosition + delta;
            var size = gridSize / 2f - 0.1f;
            var halfExtents = new Vector3(size, 1f, size);
            var numColliders = Physics.OverlapBoxNonAlloc(
                center, halfExtents, _collidersBuffer, _transform.rotation, collisionLayerMask);
            return numColliders == 0;
        }

        public void TurnRight() => TurnEulerDegrees(RightHand);

        public void TurnLeft() => TurnEulerDegrees(LeftHand);

        void TurnEulerDegrees(float eulerDirectionDelta)
        {
            if (IsRotating) return;
            _rotateFromDirection = _transform.rotation;
            _rotateTowardsDirection *= Quaternion.Euler(0f, eulerDirectionDelta, 0f);
            _rotationTime = 0f;
            TurnEvent.Invoke();
        }
    }
}
