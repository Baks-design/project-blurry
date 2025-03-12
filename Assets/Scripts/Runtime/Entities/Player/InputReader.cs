using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using static InputSystem_Actions;

namespace Assets.Scripts.Runtime.Entities.Player
{
    [CreateAssetMenu(menuName = "Data/Systems/InputReader")]
    public class InputReader : ScriptableObject, IGameplayActions
    {
        public InputSystem_Actions inputActions;

        public event UnityAction<bool> MoveForward = delegate { };
        public event UnityAction<bool> MoveRunForward = delegate { };
        public event UnityAction<bool> MoveBackward = delegate { };
        public event UnityAction<bool> MoveStrafeRight = delegate { };
        public event UnityAction<bool> MoveStrafeLeft = delegate { };
        public event UnityAction<bool> MoveTurnRight = delegate { };
        public event UnityAction<bool> MoveTurnLeft = delegate { };
        public event UnityAction<bool> HoldLook = delegate { };
        public event UnityAction<Vector2> Look = delegate { };
        public event UnityAction Interaction = delegate { };
        public event UnityAction Crouch = delegate { };
        public event UnityAction OpenMenu = delegate { };
        public event UnityAction CloseMenu = delegate { };

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
        public void OnLook(InputAction.CallbackContext context) => Look.Invoke(context.ReadValue<Vector2>());

        public void OnForward(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    MoveForward.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    MoveForward.Invoke(false);
                    break;
            }
        }

        public void OnRun(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    MoveRunForward.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    MoveRunForward.Invoke(false);
                    break;
            }
        }

        public void OnBackward(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    MoveBackward.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    MoveBackward.Invoke(false);
                    break;
            }
        }

        public void OnStrafeRight(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    MoveStrafeRight.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    MoveStrafeRight.Invoke(false);
                    break;
            }
        }

        public void OnStrafeLeft(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    MoveStrafeLeft.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    MoveStrafeLeft.Invoke(false);
                    break;
            }
        }

        public void OnTurnRight(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    MoveTurnRight.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    MoveTurnRight.Invoke(false);
                    break;
            }
        }

        public void OnTurnLeft(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    MoveTurnLeft.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    MoveTurnLeft.Invoke(false);
                    break;
            }
        }

        public void OnHoldLook(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    HoldLook.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    HoldLook.Invoke(false);
                    break;
            }
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            if (context.phase is InputActionPhase.Started)
                Crouch.Invoke();
        }

        public void OnInteraction(InputAction.CallbackContext context)
        {
            if (context.phase is InputActionPhase.Started)
                Interaction.Invoke();
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
        public void SetCursorLockState(bool isLocked) => Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
        #endregion
    }
}