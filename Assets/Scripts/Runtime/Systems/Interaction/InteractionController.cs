using System;
using Assets.Scripts.Runtime.Components.VFX;
using Assets.Scripts.Runtime.Systems.EventBus;
using Assets.Scripts.Runtime.Systems.EventBus.Events;
using Assets.Scripts.Runtime.Systems.Interaction.States;
using Assets.Scripts.Runtime.Utilities.Helpers;
using Assets.Scripts.Runtime.Utilities.Patterns.ServicesLocator;
using Assets.Scripts.Runtime.Utilities.Patterns.StateMachine;
using KBCore.Refs;
using UnityEngine;

namespace Assets.Scripts.Runtime.Systems.Interaction
{
    public class InteractionController : MonoBehaviour //       StatefulEntity
    {
        [SerializeField, Self] Transform tr;
        [SerializeField, Anywhere] InputReader inputReader;
        [SerializeField] LayerMask interactableLayer;
        [SerializeField, Range(0.5f, 5f)] float rayDistance = 2f;
        [SerializeField, Range(0.1f, 1f)] float raySphereRadius = 0.5f;
        bool lastHitSomething, pickUpInput, dropInput;
        Ray lastRay;
        Vector2 lookInput;
        Transform hitTransform, cameraTransform;
        readonly RaycastHit[] hits = new RaycastHit[1];
        IPickable pickable;
        IVolumeService volumeService;

        #region Setup
        void Awake()
        {
            //base.Awake();
            //SetupStateMachine();
            GetVars();
        }

        // void SetupStateMachine() //FIXME: Interaction States
        // {
        //     var search = new SearchState(this);
        //     var pickUp = new PickUpState(this);
        //     var drop = new DropState(this);
        //     var save = new SaveState(this);

        //     At(search, pickUp, pickUpInput && lastHitSomething);
        //     At(pickUp, save, pickUpInput && pickable != null);
        //     At(pickUp, drop, dropInput && pickable != null);
        //     At(drop, search, pickable == null);
        //     At(save, search, pickable == null);

        //     stateMachine.SetState(search);
        // }

        void GetVars() => cameraTransform = Camera.main.transform;

        void Start() => ServiceLocator.Global.Get(out volumeService);
        #endregion

        #region Inputs
        void OnEnable()
        {
            inputReader.Look += LookInput;
            inputReader.Pickup += PickupInput;
            inputReader.Drop += DropInput;
        }

        void OnDisable()
        {
            inputReader.Look -= LookInput;
            inputReader.Pickup -= PickupInput;
            inputReader.Drop -= DropInput;
        }

        void LookInput(Vector2 input, bool isC) => lookInput = input;

        void PickupInput(bool input) => pickUpInput = input;

        void DropInput(bool input) => dropInput = input;
        #endregion

        void Update()
        {
            //base.Update();
            CheckForInteractable();

            if (pickUpInput)
                OnPickUp();
            if (pickUpInput)
                OnSave();
            if (dropInput)
                OnDrop();
        }

        #region Search State
        public void CheckForInteractable()
        {
            var lastRay = new Ray(cameraTransform.position, cameraTransform.forward);
            var hitCount = Physics.SphereCastNonAlloc(
                lastRay.origin, raySphereRadius, lastRay.direction,
                hits, rayDistance, interactableLayer, QueryTriggerInteraction.Ignore);
            var lastHitSomething = hitCount > 0;
            hitTransform = lastHitSomething ? hits[0].transform : null;
        }
        #endregion

        #region Pick State
        public void OnPickUp()
        {
            if (hitTransform != null && hitTransform.TryGetComponent(out pickable))
            {
                if (pickable != null && !pickable.IsPicked)
                {
                    EventBus<PickItemEvent>.Raise(new PickItemEvent { isMoveValid = false });
                    volumeService.ToggleDepthOfField(true);
                    pickable.OnPickUp(cameraTransform.position, cameraTransform.rotation, lookInput);
                }
            }
        }

        public void UpdatePickUp() { }
        #endregion

        #region Drop State
        public void OnDrop()
        {
            if (pickable != null && pickable.IsPicked)
            {
                EventBus<PickItemEvent>.Raise(new PickItemEvent { isMoveValid = true });
                volumeService.ToggleDepthOfField(false);
                pickable.OnDrop(tr.position);
                pickable = null;
            }
        }

        public void UpdateDrop() { }
        #endregion

        #region Save State
        public void OnSave()
        {
            if (pickable != null && pickable.IsPicked)
            {
                EventBus<PickItemEvent>.Raise(new PickItemEvent { isMoveValid = true });
                volumeService.ToggleDepthOfField(false);
                pickable.OnSave(tr.position);
                //TODO: Add to inventory
            }
        }

        public void UpdateSave() { }
        #endregion
    }
}