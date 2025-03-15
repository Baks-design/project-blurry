using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using static InputSystem_Actions;

namespace Assets.Scripts.Runtime.Utilities.Helpers
{
    [CreateAssetMenu(menuName = "Data/Systems/InputReader")]
    public class InputReader : ScriptableObject, IGameplayActions
    {
        public InputSystem_Actions inputActions;

        public event UnityAction MoveForward = delegate { };
        public event UnityAction<bool> MoveRunForward = delegate { };
        public event UnityAction MoveBackward = delegate { };
        public event UnityAction MoveStrafeRight = delegate { };
        public event UnityAction MoveStrafeLeft = delegate { };
        public event UnityAction MoveTurnRight = delegate { };
        public event UnityAction MoveTurnLeft = delegate { };
        public event UnityAction<bool> HoldLook = delegate { };
        public event UnityAction<Vector2, bool> Look = delegate { };
        public event UnityAction<bool> Pickup = delegate { };
        public event UnityAction<bool> Drop = delegate { };
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
        public void OnLook(InputAction.CallbackContext context) => Look.Invoke(context.ReadValue<Vector2>(), IsDeviceMouse(context));

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
        public void SetCursorLockState(bool isLocked) => Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
        #endregion
    }
}