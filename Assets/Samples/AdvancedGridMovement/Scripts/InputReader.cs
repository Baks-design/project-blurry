using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Runtime.Components.Inputs
{
    [CreateAssetMenu(menuName = "Data/System/InputReader")]
    public class InputReader : ScriptableObject
    {
        InputActionMap player, ui;
        InputAction forward, backward, strafeRight, strafeLeft, turnRight, turnLeft, crouch;
        InputAction holdLook, look, interaction, openMenu, closeMenu;
        Gamepad gamepad;

        public bool IsKeyboardDevice { get; private set; }
        public bool IsGamepadDevice { get; private set; }

        public event Action OnForward = delegate { };
        public event Action OnBackward = delegate { };
        public event Action OnStrafeRight = delegate { };
        public event Action OnStrafeLeft = delegate { };
        public event Action OnTurnRight = delegate { };
        public event Action OnTurnLeft = delegate { };
        public event Action OnCrouch = delegate { };
        public event Action OnInteraction = delegate { };
        public event Action<bool> OnHoldLook = delegate { };
        public event Action<Vector2> OnLook = delegate { };
        public event Action OnOpenMenu = delegate { };
        public event Action OnCloseMenu = delegate { };
        Action<InputAction.CallbackContext> onHoldLookPerformed;
        Action<InputAction.CallbackContext> onLookPerformed;
        Action<InputAction.CallbackContext> onLookCanceled;

        void Awake()
        {
            FindActionMaps();
            FindActions();
            FindDevices();

            onHoldLookPerformed = context => OnHoldLook.Invoke(context.action.IsPressed());
            onLookPerformed = context => OnLook.Invoke(context.ReadValue<Vector2>());
            onLookCanceled = context => OnLook.Invoke(context.ReadValue<Vector2>());
        }

        void FindActionMaps()
        {
            player = InputSystem.actions.FindActionMap("Player");
            ui = InputSystem.actions.FindActionMap("UI");
        }

        void FindActions()
        {
            forward = player.FindAction("Forward");
            backward = player.FindAction("Backward");
            strafeRight = player.FindAction("StrafeRight");
            strafeLeft = player.FindAction("StrafeLeft");
            turnRight = player.FindAction("TurnRight");
            turnLeft = player.FindAction("TurnLeft");
            crouch = player.FindAction("Crouch");
            interaction = player.FindAction("Interaction");
            holdLook = player.FindAction("HoldLook");
            look = player.FindAction("Look");
            openMenu = ui.FindAction("OpenMenu");
            closeMenu = ui.FindAction("CloseMenu");
        }

        void FindDevices() => gamepad = Gamepad.current;

        void OnEnable()
        {
            InputSystem.onDeviceChange += OnDeviceChange;

            forward.started += _ => OnForward.Invoke();
            backward.started += _ => OnBackward.Invoke();
            strafeRight.started += _ => OnStrafeRight.Invoke();
            strafeLeft.started += _ => OnStrafeLeft.Invoke();
            turnRight.started += _ => OnTurnRight.Invoke();
            turnLeft.started += _ => OnTurnLeft.Invoke();
            crouch.started += _ => OnCrouch.Invoke();
            holdLook.performed += onHoldLookPerformed;
            look.performed += onLookPerformed;
            look.canceled += onLookCanceled;
            interaction.started += _ => OnInteraction.Invoke();
            interaction.canceled += _ => OnInteraction.Invoke();
            openMenu.started += _ => OnOpenMenu.Invoke();
            closeMenu.started += _ => OnCloseMenu.Invoke();
        }

        void OnDisable()
        {
            forward.started -= _ => OnForward.Invoke();
            backward.started -= _ => OnBackward.Invoke();
            strafeRight.started -= _ => OnStrafeRight.Invoke();
            strafeLeft.started -= _ => OnStrafeLeft.Invoke();
            turnRight.started -= _ => OnTurnRight.Invoke();
            turnLeft.started -= _ => OnTurnLeft.Invoke();
            crouch.started -= _ => OnCrouch.Invoke();
            holdLook.performed -= onHoldLookPerformed;
            look.performed -= onLookPerformed;
            look.canceled -= onLookCanceled;
            interaction.started -= _ => OnInteraction.Invoke();
            interaction.canceled -= _ => OnInteraction.Invoke();
            openMenu.started -= _ => OnOpenMenu.Invoke();
            closeMenu.started -= _ => OnCloseMenu.Invoke();

            InputSystem.onDeviceChange -= OnDeviceChange;
        }

        void OnDestroy() => InputSystem.onDeviceChange -= OnDeviceChange;

        #region Devices
        void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            switch (change)
            {
                case InputDeviceChange.Added:
                    if (device is Keyboard or Mouse)
                    {
                        IsKeyboardDevice = true;
                        Debug.Log("Keyboard/Mouse device added.");
                    }
                    else if (device is Gamepad)
                    {
                        IsGamepadDevice = true;
                        Debug.Log("Gamepad device added.");
                    }
                    break;

                case InputDeviceChange.Removed:
                    if (device is Keyboard or Mouse)
                    {
                        IsKeyboardDevice = false;
                        Debug.Log("Keyboard/Mouse device removed.");
                    }
                    else if (device is Gamepad)
                    {
                        IsGamepadDevice = false;
                        Debug.Log("Gamepad device removed.");
                    }
                    break;
            }
        }
        #endregion

        #region Maps
        public void EnablePlayerMap()
        {
            player.Enable();
            ui.Disable();
        }

        public void EnableUIMap()
        {
            player.Disable();
            ui.Enable();
        }

        public void DisableMaps()
        {
            player.Disable();
            ui.Disable();
        }
        #endregion

        #region Haptics
        public void PauseHaptics() => InputSystem.PauseHaptics();

        public void ResumeHaptics() => InputSystem.ResumeHaptics();

        public void ResetHaptics() => InputSystem.ResetHaptics();

        public async Awaitable StartRumble(float lowFrequency, float highFrequency, float duration)
        {
            if (gamepad != null)
            {
                gamepad.SetMotorSpeeds(lowFrequency, highFrequency);
                await StopRumbleAfterDelay(duration);
            }
        }

        async Awaitable StopRumbleAfterDelay(float duration)
        {
            await Awaitable.WaitForSecondsAsync(duration);
            gamepad.SetMotorSpeeds(0f, 0f);
        }
        #endregion
    }
}
//FIXME