using System;
using System.Collections.Generic;
using Assets.Scripts.Runtime.Systems.EventBus;
using Assets.Scripts.Runtime.Systems.EventBus.Events;
using Assets.Scripts.Runtime.Utilities.Helpers;
using KBCore.Refs;
using UnityEngine;

namespace Assets.Scripts.Runtime.Entities.Player
{
    [RequireComponent(typeof(PlayerMovementController))]
    public class MovementQueue : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Self] PlayerMovementController movementController;
        [SerializeField, Anywhere] InputReader inputReader;
        [Header("Queue Settings")]
        [SerializeField, Range(1, 5)] int QueueDepth = 1;
        [SerializeField, Range(0.5f, 5f)] float keyPressThresholdTime = 1f;
        bool _isRunHold;
        bool _isMoveValid;
        float _forwardKeyPressedTime;
        Queue<Action> _movementQueue;

        public event Action OnEventIfTheCommandIsNotQueable = delegate { };
        EventBinding<LockPlayerMovementEvent> _lockPlayerMovementEventBinding;

        void Awake()
        {
            inputReader.SetCursorLockState(true);
            inputReader.EnablePlayerActions();
        }

        void OnEnable()
        {
            SubInputEvents();
            SubBusEvents();
        }

        void OnDisable()
        {
            UnsubInputEvents();
            UnsubBusEvents();
        }

        void SubInputEvents()
        {
            movementController.OnBlocked += FlushQueue;
            inputReader.MoveForward += Forward;
            inputReader.MoveRunForward += RunForward;
            inputReader.MoveBackward += Backward;
            inputReader.MoveStrafeRight += StrafeRight;
            inputReader.MoveStrafeLeft += StrafeLeft;
            inputReader.MoveTurnRight += TurnRight;
            inputReader.MoveTurnLeft += TurnLeft;
        }

        void SubBusEvents()
        {
            _lockPlayerMovementEventBinding = new EventBinding<LockPlayerMovementEvent>(HandleLockPlayerMovementEvent);
            EventBus<LockPlayerMovementEvent>.Register(_lockPlayerMovementEventBinding);
        }

        void UnsubInputEvents()
        {
            movementController.OnBlocked -= FlushQueue;
            inputReader.MoveForward -= Forward;
            inputReader.MoveRunForward -= RunForward;
            inputReader.MoveBackward -= Backward;
            inputReader.MoveStrafeRight -= StrafeRight;
            inputReader.MoveStrafeLeft -= StrafeLeft;
            inputReader.MoveTurnRight -= TurnRight;
            inputReader.MoveTurnLeft -= TurnLeft;
        }

        void UnsubBusEvents() => EventBus<LockPlayerMovementEvent>.Deregister(_lockPlayerMovementEventBinding);

        void HandleLockPlayerMovementEvent(LockPlayerMovementEvent ev)
        {
            _isMoveValid = ev.isMoveValid;
            _movementQueue.Clear();
        }

        void RunForward(bool isRun) => _isRunHold = isRun;
        void Forward() => QueueCommand(() => movementController.MoveForward());
        void Backward() => QueueCommand(() => movementController.MoveBackward());
        void StrafeRight() => QueueCommand(() => movementController.StrafeRight());
        void StrafeLeft() => QueueCommand(() => movementController.StrafeLeft());
        void TurnRight() => QueueCommand(() => movementController.TurnRight());
        void TurnLeft() => QueueCommand(() => movementController.TurnLeft());

        void Start()
        {
            InitQueue();
            InitVars();
            ResetKeyPressTimer();
        }

        void InitQueue() => _movementQueue = new Queue<Action>(QueueDepth);

        void InitVars()
        {
            _isRunHold = false;
            _isMoveValid = true;
            _forwardKeyPressedTime = 0f;
        }

        void Update()
        {
            MovementHandle();
            RunMovementHandle();
        }

        void MovementHandle()
        {
            // Skip if movement is not valid or the player is not stationary
            if (!_isMoveValid || !movementController.IsStationary || _movementQueue.Count <= 0) return;

            // Execute the next movement command in the queue
            var action = _movementQueue.Dequeue();
            action?.Invoke();
        }

        void RunMovementHandle()
        {
            if (!_isMoveValid) return;

            if (_isRunHold)
                RunForward();
            else
                StopRunForward();
        }

        void RunForward()
        {
            // Track how long the forward key has been pressed
            _forwardKeyPressedTime += Time.deltaTime;

            // If the threshold is reached and the queue has space, switch to running and queue a forward movement
            if (_forwardKeyPressedTime >= keyPressThresholdTime && _movementQueue.Count < QueueDepth)
            {
                movementController.SwitchToRunning();
                QueueCommand(() => movementController.MoveForward());
            }
        }

        void QueueCommand(Action action)
        {
            // If the queue is full, trigger the event and skip adding the command
            if (_movementQueue.Count >= QueueDepth)
            {
                OnEventIfTheCommandIsNotQueable.Invoke();
                return;
            }

            // Add the command to the queue
            _movementQueue.Enqueue(action);
        }

        void StopRunForward()
        {
            if (_forwardKeyPressedTime < keyPressThresholdTime) return;

            // Flush the queue and reset the timer
            FlushQueue();
        }

        void FlushQueue()
        {
            // Reset the queue and switch back to walking
            ResetKeyPressTimer();
            _movementQueue.Clear();
            movementController.SwitchToWalking();
        }

        void ResetKeyPressTimer() => _forwardKeyPressedTime = 0f;
    }
}