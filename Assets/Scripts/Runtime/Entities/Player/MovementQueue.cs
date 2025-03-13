using System;
using System.Collections.Generic;
using Assets.Scripts.Runtime.Systems.EventBus;
using Assets.Scripts.Runtime.Systems.EventBus.Events;
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
        [SerializeField, Range(0.5f, 5f)] float keyPressThresholdTime = 1f;
        bool isRunHold, isMoveValid = true;
        float forwardKeyPressedTime;
        Queue<Action> movementQueue;

        public event Action OnEventIfTheCommandIsNotQueable = delegate { };
        EventBinding<PickItemEvent> pickItemEventBinding;

        void Awake()
        {
            inputReader.SetCursorLockState(true);
            inputReader.EnablePlayerActions();
        }

        void OnEnable()
        {
            movementController.BlockedEvent += FlushQueue;
            inputReader.MoveForward += Forward;
            inputReader.MoveRunForward += RunForward;
            inputReader.MoveBackward += Backward;
            inputReader.MoveStrafeRight += StrafeRight;
            inputReader.MoveStrafeLeft += StrafeLeft;
            inputReader.MoveTurnRight += TurnRight;
            inputReader.MoveTurnLeft += TurnLeft;

            pickItemEventBinding = new EventBinding<PickItemEvent>(HandlePickItemEvent);
            EventBus<PickItemEvent>.Register(pickItemEventBinding);
        }

        void OnDisable()
        {
            movementController.BlockedEvent -= FlushQueue;
            inputReader.MoveForward -= Forward;
            inputReader.MoveRunForward -= RunForward;
            inputReader.MoveBackward -= Backward;
            inputReader.MoveStrafeRight -= StrafeRight;
            inputReader.MoveStrafeLeft -= StrafeLeft;
            inputReader.MoveTurnRight -= TurnRight;
            inputReader.MoveTurnLeft -= TurnLeft;

            EventBus<PickItemEvent>.Deregister(pickItemEventBinding);
        }

        void Forward() => QueueCommand(() => movementController.MoveForward());
        void Backward() => QueueCommand(() => movementController.MoveBackward());
        void StrafeRight() => QueueCommand(() => movementController.StrafeRight());
        void StrafeLeft() => QueueCommand(() => movementController.StrafeLeft());
        void TurnRight() => QueueCommand(() => movementController.TurnRight());
        void TurnLeft() => QueueCommand(() => movementController.TurnLeft());
        void RunForward(bool isRun) => isRunHold = isRun;
        void HandlePickItemEvent(PickItemEvent pickItemEvent) => isMoveValid = pickItemEvent.isMoveValid;

        void Start()
        {
            InitQueue();
            ResetKeyPressTimer();
        }

        void InitQueue() => movementQueue = new Queue<Action>(QueueDepth);

        void Update()
        {
            MovementHandle();
            RunMovementHandle();
        }

        void MovementHandle()
        {
            if (!isMoveValid) return;
            if (!movementController.IsStationary || movementQueue.Count <= 0) return;
            var action = movementQueue.Dequeue();
            action?.Invoke();
        }

        void RunMovementHandle()
        {
            if (!isMoveValid) return;
            if (isRunHold)
                RunForward();
            else if (!isRunHold)
                StopRunForward();
        }

        void RunForward()
        {
            forwardKeyPressedTime += Time.deltaTime;
            if (forwardKeyPressedTime >= keyPressThresholdTime && movementQueue.Count < QueueDepth)
            {
                movementController.SwitchToRunning();
                QueueCommand(() => movementController.MoveForward());
            }
        }

        void QueueCommand(Action action)
        {
            if (movementQueue.Count >= QueueDepth)
            {
                OnEventIfTheCommandIsNotQueable.Invoke();
                return;
            }
            movementQueue.Enqueue(action);
        }

        void StopRunForward()
        {
            if (forwardKeyPressedTime < keyPressThresholdTime) return;
            FlushQueue();
        }

        void FlushQueue()
        {
            ResetKeyPressTimer();
            movementQueue.Clear();
            movementController.SwitchToWalking();
        }

        void ResetKeyPressTimer() => forwardKeyPressedTime = 0f;
    }
}