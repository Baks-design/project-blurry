using System;
using Game.Runtime.Components.Inputs;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

// public class InputHandler : MonoBehaviour
// {
//     [SerializeField] EventMapping[] eventMappings;
//     [SerializeField] EventMapping[] eventMappingsKeyDown;
//     [SerializeField] EventMapping[] eventMappingsKeyUp;
//     [Serializable]
//     public struct EventMapping
//     {
//         public Key key;
//         public UnityEvent callback;
//     }
//     Keyboard keyboard;
//     Action<EventMapping> inputMappingAction;
//     Action<EventMapping> inputMappingKeyDownAction;
//     Action<EventMapping> inputMappingKeyUpAction;

//     void Start() => InputSetup();

//     void InputSetup()
//     {
//         Cursor.lockState = CursorLockMode.Locked;
//         keyboard = Keyboard.current;
//         inputMappingAction = InputMapping;
//         inputMappingKeyDownAction = InputMappingKeyDown;
//         inputMappingKeyUpAction = InputMappingKeyUp;
//     }

//     void Update() => UpdateInputs();

//     void UpdateInputs()
//     {
//         Array.ForEach(eventMappingsKeyDown, inputMappingKeyDownAction);
//         Array.ForEach(eventMappingsKeyUp, inputMappingKeyUpAction);
//         Array.ForEach(eventMappings, inputMappingAction);
//     }

//     void InputMapping(EventMapping eventMapping)
//     {
//         if (!keyboard[eventMapping.key].isPressed) return;
//         eventMapping.callback.Invoke();
//     }

//     void InputMappingKeyDown(EventMapping eventMapping)
//     {
//         if (!keyboard[eventMapping.key].wasPressedThisFrame) return;
//         eventMapping.callback.Invoke();
//     }

//     void InputMappingKeyUp(EventMapping eventMapping)
//     {
//         if (!keyboard[eventMapping.key].wasReleasedThisFrame) return;
//         eventMapping.callback.Invoke();
//     }
// }

[RequireComponent(typeof(MovementQueue))]
public class InputHandler : MonoBehaviour
{
    [SerializeField] InputReader inputReader;
    [SerializeField, Self] MovementQueue movementQueue;

    void Start() => Cursor.lockState = CursorLockMode.Locked; //TODO: Change to another script

    void OnEnable()
    {
        inputReader.OnForward += PerformedForward;
        inputReader.OnBackward += PerformedBackward;
        inputReader.OnStrafeRight += PerformedStrafeRight;
        inputReader.OnStrafeLeft += PerformedStrafeLeft;
        inputReader.OnTurnRight += PerformedTurnRight;
        inputReader.OnTurnLeft += PerformedTurnLeft;
    }

    void OnDisable()
    {
        inputReader.OnForward -= PerformedForward;
        inputReader.OnBackward -= PerformedBackward;
        inputReader.OnStrafeRight -= PerformedStrafeRight;
        inputReader.OnStrafeLeft -= PerformedStrafeLeft;
        inputReader.OnTurnRight -= PerformedTurnRight;
        inputReader.OnTurnLeft -= PerformedTurnLeft;
    }

    void PerformedForward() => movementQueue.Forward();

    void PerformedBackward() => movementQueue.Backward();

    void PerformedStrafeRight() => movementQueue.StrafeRight();

    void PerformedStrafeLeft() => movementQueue.StrafeLeft();

    void PerformedTurnRight() => movementQueue.TurnRight();

    void PerformedTurnLeft() => movementQueue.TurnLeft();
}