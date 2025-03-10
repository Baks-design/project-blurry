using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using KBCore.Refs;

public class MovementQueue : MonoBehaviour
{
    [SerializeField, Range(1, 5)] int QueueDepth = 1;
    [SerializeField] float keyPressThresholdTime = 1f;
    [SerializeField] UnityEvent EventIfTheCommandIsNotQueable;
    [SerializeField, Self, AssetPreview(useLabel: false)] AdvancedGridMovement advancedGridMovement;
    float forwardKeyPressedTime;
    Queue<Action> movementQueue;

    void Start()
    {
        InitQueue();
        ResetKeyPressTimer();
    }

    void InitQueue() => movementQueue = new Queue<Action>(QueueDepth);

    void Update() => MovementHandle();

    void MovementHandle()
    {
        if (!advancedGridMovement.IsStationary || movementQueue.Count <= 0) return;
        var action = movementQueue.Dequeue();
        action?.Invoke();
    }

    public void Forward() => QueueCommand(() => advancedGridMovement.MoveForward());

    public void Backward() => QueueCommand(() => advancedGridMovement.MoveBackward());

    public void StrafeLeft() => QueueCommand(() => advancedGridMovement.StrafeLeft());

    public void StrafeRight() => QueueCommand(() => advancedGridMovement.StrafeRight());

    public void TurnLeft() => QueueCommand(() => advancedGridMovement.TurnLeft());

    public void TurnRight() => QueueCommand(() => advancedGridMovement.TurnRight());

    public void RunForward()
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
            EventIfTheCommandIsNotQueable?.Invoke();
        else
            movementQueue.Enqueue(action);
    }

    public void StopRunForward()
    {
        if (forwardKeyPressedTime < keyPressThresholdTime) return;
        FlushQueue();
    }

    public void FlushQueue()
    {
        ResetKeyPressTimer();
        movementQueue.Clear();
        advancedGridMovement.SwitchToWalking();
    }

    void ResetKeyPressTimer() => forwardKeyPressedTime = 0f;
}
//TODO: using c# events instead unity events