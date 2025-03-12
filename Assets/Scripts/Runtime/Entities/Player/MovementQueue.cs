using System;
using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;

namespace Assets.Scripts.Runtime.Entities.Player
{
    [RequireComponent(typeof(AdvancedGridMovement))]
    public class MovementQueue : MonoBehaviour
    {
        [SerializeField, Self] AdvancedGridMovement advancedGridMovement;
        [SerializeField, Anywhere] InputReader inputReader;
        [SerializeField, Range(1, 5)] int QueueDepth = 1;
        [SerializeField, Range(0.5f, 5f)] float keyPressThresholdTime = 1f;
        bool IsMoveForward, IsMoveRunForward, IsMoveBackward, IsMoveStrafeRight;
        bool IsMoveStrafeLeft, IsMoveTurnRight, IsMoveTurnLeft;
        float forwardKeyPressedTime;
        Queue<Action> movementQueue;

        public event Action OnEventIfTheCommandIsNotQueable = delegate { };

        void Awake() => inputReader.EnablePlayerActions();

        void OnEnable()
        {
            advancedGridMovement.BlockedEvent += FlushQueue;
            inputReader.MoveForward += PerformedForward;
            inputReader.MoveRunForward += PerformedRunForward;
            inputReader.MoveBackward += PerformedBackward;
            inputReader.MoveStrafeRight += PerformedStrafeRight;
            inputReader.MoveStrafeLeft += PerformedStrafeLeft;
            inputReader.MoveTurnRight += PerformedTurnRight;
            inputReader.MoveTurnLeft += PerformedTurnLeft;
        }

        void OnDisable()
        {
            advancedGridMovement.BlockedEvent -= FlushQueue;
            inputReader.MoveForward -= PerformedForward;
            inputReader.MoveRunForward -= PerformedRunForward;
            inputReader.MoveBackward -= PerformedBackward;
            inputReader.MoveStrafeRight -= PerformedStrafeRight;
            inputReader.MoveStrafeLeft -= PerformedStrafeLeft;
            inputReader.MoveTurnRight -= PerformedTurnRight;
            inputReader.MoveTurnLeft -= PerformedTurnLeft;
        }

        void PerformedForward(bool value) => IsMoveForward = value;
        void PerformedRunForward(bool value) => IsMoveRunForward = value;
        void PerformedBackward(bool value) => IsMoveBackward = value;
        void PerformedStrafeRight(bool value) => IsMoveStrafeRight = value;
        void PerformedStrafeLeft(bool value) => IsMoveStrafeLeft = value;
        void PerformedTurnRight(bool value) => IsMoveTurnRight = value;
        void PerformedTurnLeft(bool value) => IsMoveTurnLeft = value;

        void Start()
        {
            InitQueue();
            ResetKeyPressTimer();
            inputReader.SetCursorLockState(true);
        }

        void InitQueue() => movementQueue = new Queue<Action>(QueueDepth);

        void Update()
        {
            MovementHandle();
            HandleQueueCommand();
        }

        void MovementHandle()
        {
            if (!advancedGridMovement.IsStationary || movementQueue.Count <= 0) return;
            var action = movementQueue.Dequeue();
            action?.Invoke();
        }

        void HandleQueueCommand()
        {
            if (IsMoveForward) QueueCommand(() => advancedGridMovement.MoveForward());
            if (IsMoveForward && IsMoveRunForward) RunForward();
            else if (IsMoveForward && !IsMoveRunForward) StopRunForward();
            if (IsMoveBackward) QueueCommand(() => advancedGridMovement.MoveBackward());
            if (IsMoveStrafeRight) QueueCommand(() => advancedGridMovement.StrafeRight());
            if (IsMoveStrafeLeft) QueueCommand(() => advancedGridMovement.StrafeLeft());
            if (IsMoveTurnRight) QueueCommand(() => advancedGridMovement.TurnRight());
            if (IsMoveTurnLeft) QueueCommand(() => advancedGridMovement.TurnLeft());
        }

        void RunForward()
        {
            forwardKeyPressedTime += Time.deltaTime;
            if (forwardKeyPressedTime >= keyPressThresholdTime && movementQueue.Count < QueueDepth)
            {
                advancedGridMovement.SwitchToRunning();
                QueueCommand(() => advancedGridMovement.MoveForward());
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
            advancedGridMovement.SwitchToWalking();
        }

        void ResetKeyPressTimer() => forwardKeyPressedTime = 0f;
    }
}