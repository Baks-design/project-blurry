using UnityEngine;
using UnityEngine.InputSystem;
using static InputSystem_Actions;
using System;

namespace Assets.Scripts.Runtime.Utilities.Helpers
{
    [CreateAssetMenu(menuName = "Data/Systems/InputReader")]
    public class InputReader : ScriptableObject, IGameplayActions
    {
        public InputSystem_Actions inputActions;

        //Move
        public event Action MoveForward = delegate { };
        public event Action<bool> MoveRunForward = delegate { };
        public event Action MoveBackward = delegate { };
        public event Action MoveStrafeRight = delegate { };
        public event Action MoveStrafeLeft = delegate { };
        public event Action MoveTurnRight = delegate { };
        public event Action MoveTurnLeft = delegate { };
        public event Action Crouch = delegate { };

        //Look
        public event Action<Vector2, bool> Look = delegate { };
        public event Action LookMode = delegate { };
        public event Action<bool> LookInteractionDown = delegate { };
        public event Action<bool> LookInteractionHold = delegate { };
        public event Action SquintDown = delegate { };
        public event Action<bool> SquintHold = delegate { };

        //Actions
        public event Action<bool> Pickup = delegate { };
        public event Action<bool> Drop = delegate { };

        //UI
        public event Action OpenMenu = delegate { };
        public event Action CloseMenu = delegate { };

        public void EnablePlayerActions()
        {
            if (inputActions == null)
            {
                inputActions = new InputSystem_Actions();
                inputActions.Gameplay.SetCallbacks(this);
            }
            inputActions.Enable();
        }

        #region Event Handlers
        public void OnLook(InputAction.CallbackContext context)
        => Look.Invoke(context.ReadValue<Vector2>(), IsDeviceMouse(context));

        bool IsDeviceMouse(InputAction.CallbackContext context) => context.control.device.name == "Mouse";

        public void OnForward(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    MoveForward.Invoke();
                    break;
                case InputActionPhase.Performed:
                    MoveRunForward.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    MoveRunForward.Invoke(false);
                    break;
            }
        }

        public void OnBackward(InputAction.CallbackContext context)
        {
            if (context.phase is InputActionPhase.Started)
                MoveBackward.Invoke();
        }

        public void OnStrafeRight(InputAction.CallbackContext context)
        {
            if (context.phase is InputActionPhase.Started)
                MoveStrafeRight.Invoke();
        }

        public void OnStrafeLeft(InputAction.CallbackContext context)
        {
            if (context.phase is InputActionPhase.Started)
                MoveStrafeLeft.Invoke();
        }

        public void OnTurnRight(InputAction.CallbackContext context)
        {
            if (context.phase is InputActionPhase.Started)
                MoveTurnRight.Invoke();
        }

        public void OnTurnLeft(InputAction.CallbackContext context)
        {
            if (context.phase is InputActionPhase.Started)
                MoveTurnLeft.Invoke();
        }

        public void OnLookInteraction(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    LookInteractionDown.Invoke(true);
                    break;
                case InputActionPhase.Performed:
                    LookInteractionHold.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    LookInteractionDown.Invoke(false);
                    LookInteractionHold.Invoke(false);
                    break;
            }
        }

        public void OnSquint(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    SquintHold.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    SquintHold.Invoke(false);
                    break;
            }
        }

        public void OnLookMode(InputAction.CallbackContext context)
        {
            if (context.phase is InputActionPhase.Started)
                LookMode.Invoke();
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            if (context.phase is InputActionPhase.Started)
                Crouch.Invoke();
        }

        public void OnPickUp(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Pickup.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Pickup.Invoke(false);
                    break;
            }
        }

        public void OnDrop(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Drop.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Drop.Invoke(false);
                    break;
            }
        }

        public void OnOpenMenu(InputAction.CallbackContext context)
        {
            if (context.phase is InputActionPhase.Started)
                OpenMenu.Invoke();
        }

        public void OnCloseMenu(InputAction.CallbackContext context)
        {
            if (context.phase is InputActionPhase.Started)
                CloseMenu.Invoke();
        }
        #endregion

        #region Misc
        public void SetCursorLockState(bool isLocked)
        => Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;

        public static void SetCursorState(bool lockCursor)
        {
#if UNITY_STANDALONE
            Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
#elif UNITY_EDITOR
            Cursor.lockState = lockCursor ? CursorLockMode.Confined : CursorLockMode.None;
#endif
        }
        #endregion
    }
}