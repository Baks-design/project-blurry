using System;
using Assets.Scripts.Runtime.Systems.EventBus;
using Assets.Scripts.Runtime.Systems.EventBus.Events;
using Assets.Scripts.Runtime.Systems.Interaction.States;
using Assets.Scripts.Runtime.Utilities.Helpers;
using Assets.Scripts.Runtime.Utilities.Patterns.StateMachine;
using KBCore.Refs;
using UnityEngine;

namespace Assets.Scripts.Runtime.Systems.Interaction
{
    public class InteractionController : StatefulEntity
    {
        [SerializeField, Self] Transform tr;
        [SerializeField, Anywhere] VolumeController volumeController; //TODO: Add VContainer
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

        #region Setup
        protected override void Awake()
        {
            base.Awake();
            SetupStateMachine();
            GetVars();
        }

        void SetupStateMachine()
        {
            var search = new SearchState(this);
            var pickUp = new PickUpState(this);
            var drop = new DropState(this);
            var save = new SaveState(this);

            // At(search, pickUp, pickUpInput && lastHitSomething);

            // At(pickUp, save, pickUpInput && pickable != null);
            // At(pickUp, drop, dropInput && pickable != null);

            // At(drop, search, pickable == null);
            // At(save, search, pickable == null);

            stateMachine.SetState(search);
        }

        void GetVars() => cameraTransform = Camera.main.transform;
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

        #region Search State
        public void CheckForInteractable()
        {
            lastRay = new Ray(cameraTransform.position, cameraTransform.forward);
            var hitCount = Physics.SphereCastNonAlloc(
                lastRay.origin, raySphereRadius, lastRay.direction,
                hits, rayDistance, interactableLayer, QueryTriggerInteraction.Ignore);
            lastHitSomething = hitCount > 0;
            hitTransform = lastHitSomething ? hits[0].transform : null;
        }
        #endregion

        #region Pick State
        public void OnPickUp()
        {
            if (hitTransform == null || !hitTransform.TryGetComponent(out pickable)) return;

            EventBus<PickItemEvent>.Raise(new PickItemEvent { isMoveValid = false });
            volumeController.ToggleDepthOfField(true);
        }

        public void UpdatePickUp() => pickable?.OnPickUp(cameraTransform.position, cameraTransform.rotation, lookInput);
        #endregion

        #region Drop State
        public void OnDrop()
        {
            EventBus<PickItemEvent>.Raise(new PickItemEvent { isMoveValid = true });
            volumeController.ToggleDepthOfField(false);
        }

        public void UpdateDrop() => pickable?.OnDrop(tr.position);
        #endregion

        #region Save State
        public void OnSave()
        {
            EventBus<PickItemEvent>.Raise(new PickItemEvent { isMoveValid = true });
            volumeController.ToggleDepthOfField(false);
            //TODO: Inventory
        }

        public void UpdateSave() => pickable?.OnSave(tr.position);
        #endregion

        #region Debug
        void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
            Gizmos.color = lastHitSomething ? Color.red : Color.green;
            Gizmos.DrawLine(lastRay.origin, lastRay.origin + lastRay.direction * rayDistance);
            Gizmos.DrawWireSphere(lastRay.origin + lastRay.direction * rayDistance, raySphereRadius);
        }
        #endregion
    }
}