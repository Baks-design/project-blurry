using System;
using Assets.Scripts.Runtime.Entities.Player.States;
using Assets.Scripts.Runtime.Utilities.Patterns.StateMachine;
using KBCore.Refs;
using UnityEngine;

namespace Assets.Scripts.Runtime.Entities.Player
{
    public class PlayerMovementController : StatefulEntity
    {
        [Header("References")]
        [SerializeField, Self] Transform _transform; 
        [Header("Grid Settings")]
        [SerializeField] LayerMask collisionLayerMask; 
        [SerializeField, Range(1f, 5f)] float gridSize = 3f; 
        [Header("Walk Settings")]
        [SerializeField, Range(1f, 5f)] float walkSpeed = 1f; 
        [SerializeField, Range(1f, 10f)] float turnSpeed = 5f; 
        [SerializeField] AnimationCurve walkSpeedCurve; 
        [SerializeField] AnimationCurve walkHeadBobCurve; 
        [Header("Run Settings")]
        [SerializeField, Range(1f, 5f)] float runningSpeed = 1.5f; 
        [SerializeField] AnimationCurve runningSpeedCurve; 
        [SerializeField] AnimationCurve runningHeadBobCurve; 
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

        public event Action OnBlocked = delegate { }; 
        public event Action OnStep = delegate { }; 
        public event Action OnTurn = delegate { }; 
        public event Action OnRun = delegate { };

        #region Initialization
        protected override void Awake()
        {
            base.Awake();
            SetupStateMachine();
            InitializeVariables();
        }

        void SetupStateMachine()
        {
            var idle = new IdleState(this);
            var moving = new MovingState(this);

            At(idle, moving, !IsStationary);
            At(moving, idle, IsStationary);

            stateMachine.SetState(idle);
        }

        void InitializeVariables()
        {
            _moveTowardsPosition = _transform.position;
            _rotateTowardsDirection = _transform.rotation;
            _currentAnimationCurve = walkSpeedCurve;
            _currentHeadBobCurve = walkHeadBobCurve;
            _currentSpeed = walkSpeed;
            _stepTime = 1f / gridSize; // Calculate step time based on grid size
        }
        #endregion

        #region Movement Settings
        public void SwitchToWalking() => UpdateMovementSettings(walkSpeedCurve, walkHeadBobCurve, walkSpeed);

        public void SwitchToRunning()
        {
            UpdateMovementSettings(runningSpeedCurve, runningHeadBobCurve, runningSpeed);
            OnRun.Invoke(); 
        }

        void UpdateMovementSettings(AnimationCurve newCurve, AnimationCurve newHeadBobCurve, float newSpeed)
        {
            // Smoothly transition between movement settings
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
            // Find the time on the curve for a given value
            var result = 1f;
            while (position < curve.Evaluate(result) && result > 0f)
                result -= ApproximationThreshold;
            return result;
        }
        #endregion

        #region Animation and Movement
        public void AnimateRotation()
        {
            if (!IsRotating) return;

            _rotationTime += Time.deltaTime;
            _transform.rotation = Quaternion.Slerp(_rotateFromDirection, _rotateTowardsDirection, _rotationTime * turnSpeed);
            CompensateRoundingErrors();
        }

        public void AnimateMovement()
        {
            if (!IsMoving) return;

            // Update movement and step timing
            _curveTime += Time.deltaTime * _currentSpeed;
            _stepTimeCounter += Time.deltaTime * _currentSpeed;

            if (_stepTimeCounter > _stepTime)
            {
                _stepTimeCounter = 0f;
                OnStep.Invoke(); // Trigger step event
            }

            // Calculate new position and apply head bobbing
            var currentPositionValue = _currentAnimationCurve.Evaluate(_curveTime);
            var currentHeadBobValue = _currentHeadBobCurve.Evaluate(_curveTime * gridSize);

            var heightVariance = HeightInvariantVector(_moveTowardsPosition) - HeightInvariantVector(_moveFromPosition);
            var targetHeading = Vector3.Normalize(heightVariance);
            var newPosition = _moveFromPosition + targetHeading * (currentPositionValue * gridSize);
            newPosition.y = maximumStepHeight;

            // Adjust height based on raycast
            if (Physics.Raycast(newPosition, -Vector3.up, out var hit))
                newPosition.y = maximumStepHeight - hit.distance + currentHeadBobValue;
            else
                newPosition.y = currentHeadBobValue;

            _transform.position = newPosition;

            CompensateRoundingErrors();
        }

        void CompensateRoundingErrors()
        {
            // Correct rounding errors in rotation and position
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
        #endregion

        #region Movement Commands
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
                OnBlocked.Invoke(); 
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
        #endregion

        #region Rotation Commands
        public void TurnRight() => TurnEulerDegrees(RightHand);

        public void TurnLeft() => TurnEulerDegrees(LeftHand);

        void TurnEulerDegrees(float eulerDirectionDelta)
        {
            if (IsRotating) return;

            _rotateFromDirection = _transform.rotation;
            _rotateTowardsDirection *= Quaternion.Euler(0f, eulerDirectionDelta, 0f);
            _rotationTime = 0f;
            OnTurn.Invoke();
        }
        #endregion
    }
}