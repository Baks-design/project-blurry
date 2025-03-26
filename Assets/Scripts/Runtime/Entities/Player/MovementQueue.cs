using System;
using System.Collections.Generic;
using Assets.Scripts.Runtime.Utilities.Helpers;
using KBCore.Refs;
using UnityEngine;

namespace Assets.Scripts.Runtime.Entities.Player
{
    [RequireComponent(typeof(PlayerMovementController))]
    public class MovementQueue : MonoBehaviour
    {
        [SerializeField, Self] PlayerMovementController movementController;
        [SerializeField, Anywhere] InputReader inputReader;
        [SerializeField, Range(1, 5)] int QueueDepth = 1;
        [SerializeField, Range(0.5f, 5f)] float keyPressThresholdTime = 0f;
        bool _isRunHold;
        float _forwardKeyPressedTime;

        public Queue<Action> MovementQueueAction { get; private set; }

        public event Action OnEventIfTheCommandIsNotQueable = delegate { };

        void Awake()
        {
            inputReader.SetCursorLockState(true);
            inputReader.EnablePlayerActions();
        }

        void OnEnable()
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

        void OnDisable()
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

        void InitQueue() => MovementQueueAction = new Queue<Action>(QueueDepth);

        void InitVars()
        {
            _isRunHold = false;
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
            if (!movementController.IsMoveValid || !movementController.IsStationary || MovementQueueAction.Count <= 0) return;

            // Execute the next movement command in the queue
            var action = MovementQueueAction.Dequeue();
            action?.Invoke();
        }

        void RunMovementHandle()
        {
            if (!movementController.IsMoveValid) return;

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
            if (_forwardKeyPressedTime >= keyPressThresholdTime && MovementQueueAction.Count < QueueDepth)
            {
                movementController.SwitchToRunning();
                QueueCommand(() => movementController.MoveForward());
            }
        }

        void QueueCommand(Action action)
        {
            // If the queue is full, trigger the event and skip adding the command
            if (MovementQueueAction.Count >= QueueDepth)
            {
                OnEventIfTheCommandIsNotQueable.Invoke();
                return;
            }

            // Add the command to the queue
            MovementQueueAction.Enqueue(action);
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
            MovementQueueAction.Clear();
            movementController.SwitchToWalking();
        }

        void ResetKeyPressTimer() => _forwardKeyPressedTime = 0f;
    }
}