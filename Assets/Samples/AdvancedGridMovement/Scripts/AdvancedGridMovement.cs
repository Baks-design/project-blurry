using KBCore.Refs;
using UnityEngine;
using UnityEngine.Events;

public class AdvancedGridMovement : MonoBehaviour
{
    [SerializeField, Self] Transform _transform;
    [Header("Grid Settings")]
    [SerializeField] LayerMask collisionLayerMask;
    [SerializeField] float gridSize = 3f;
    [Header("Walk Settings")]
    [SerializeField] float walkSpeed = 1f;
    [SerializeField] float turnSpeed = 5f;
    [SerializeField] AnimationCurve walkSpeedCurve; //[AnimationCurveSettings(-2, -2, 2, 2, HexColor = "#FFD666")]
    [SerializeField] AnimationCurve walkHeadBobCurve; //[AnimationCurveSettings(-2, -2, 2, 2, HexColor = "#FFD666")]
    [Header("Run Settings")]
    [SerializeField] float runningSpeed = 1.5f;
    [SerializeField] AnimationCurve runningSpeedCurve; //[AnimationCurveSettings(-2, -2, 2, 2, HexColor = "#FFD666")]
    [SerializeField] AnimationCurve runningHeadBobCurve; //[AnimationCurveSettings(-2, -2, 2, 2, HexColor = "#FFD666")]
    [Header("Step Settings")]
    [SerializeField] float maximumStepHeight = 2f;
    [Header("Events")]
    [SerializeField] UnityEvent blockedEvent;
    [SerializeField] UnityEvent stepEvent;
    [SerializeField] UnityEvent turnEvent;
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
            stepEvent?.Invoke();
        }

        var currentPositionValue = _currentAnimationCurve.Evaluate(_curveTime);
        var currentHeadBobValue = _currentHeadBobCurve.Evaluate(_curveTime * gridSize);

        var heightVariance = HeightInvariantVector(_moveTowardsPosition) - HeightInvariantVector(_moveFromPosition);
        var targetHeading = Vector3.Normalize(heightVariance);
        var newPosition = _moveFromPosition + targetHeading * (currentPositionValue * gridSize);
        newPosition.y = maximumStepHeight;

        if (Physics.Raycast(new Ray(newPosition, -Vector3.up), out var hit))
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
        if (IsStationary)
        {
            var targetPosition = _moveTowardsPosition + movementDirection;
            if (FreeSpace(targetPosition))
            {
                _moveFromPosition = _transform.position;
                _moveTowardsPosition = targetPosition;
            }
            else
                blockedEvent?.Invoke();
        }
    }

    bool FreeSpace(Vector3 targetPosition)
    {
        var delta = (targetPosition - _moveTowardsPosition) * 0.6f;
        var center = _moveTowardsPosition + delta;
        var halfExtents = new Vector3(gridSize / 2f - 0.1f, 1f, gridSize / 2f - 0.1f);
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
        turnEvent?.Invoke();
    }
}
//TODO: test with settings 
//TODO: Change events