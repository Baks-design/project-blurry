using System;
using UnityEngine;
using KBCore.Refs;
using Assets.Scripts.Runtime.Components.VFX;
using Assets.Scripts.Runtime.Systems.Interaction.States;
using Assets.Scripts.Runtime.Utilities.Helpers;
using Assets.Scripts.Runtime.Utilities.Patterns.ServicesLocator;
using Assets.Scripts.Runtime.Utilities.Patterns.StateMachine;

namespace Assets.Scripts.Runtime.Systems.Interaction
{
    public class InteractionController : StatefulEntity
    {
        [SerializeField] Transform holdPosition;
        [SerializeField, Anywhere] InputReader inputReader;
        [SerializeField] LayerMask interactableLayer;
        [SerializeField, Range(0.5f, 5f)] float rayDistance = 2f;
        [SerializeField, Range(0.05f, 0.5f)] float raySphereRadius = 0.1f;
        bool pickUpInput;
        bool dropInput;
        Transform cameraTransform;
        Transform getSomething;
        RaycastHit[] hits;
        IPickable getPickable;

        protected override void Awake()
        {
            base.Awake();

            var search = new InteractionSearchState(this);
            var pickUp = new InteractionPickUpState(this);
            var drop = new InteractionDropState(this);
            var save = new InteractionSaveState(this);

            At(search, pickUp, () => pickUpInput && getPickable != null);

            At(pickUp, drop, () => dropInput);
            At(pickUp, save, () => pickUpInput);

            At(drop, search, () => true);
            At(save, search, () => true);

            stateMachine.SetState(search);
        }

        void Start()
        {
            pickUpInput = false;
            dropInput = false;
            cameraTransform = Camera.main.transform;
            getSomething = null;
            hits = new RaycastHit[1];
        }

        void OnEnable()
        {
            inputReader.Pickup += PickupInput;
            inputReader.Drop += DropInput;
        }

        void OnDisable()
        {
            inputReader.Pickup -= PickupInput;
            inputReader.Drop -= DropInput;
        }

        void PickupInput(bool input) => pickUpInput = input;

        void DropInput(bool input) => dropInput = input;

        public void OnCheck()
        {
            var hitCount = Physics.SphereCastNonAlloc(
                cameraTransform.position, raySphereRadius, cameraTransform.forward,
                hits, rayDistance, interactableLayer, QueryTriggerInteraction.Ignore);

            var lastHitSomething = hitCount > 0;
            getSomething = lastHitSomething ? hits[0].transform : null;
            if (getSomething != null)
            {
                getSomething.TryGetComponent(out getPickable);
                getPickable?.OnHoverStart();
            }
        }

        public void OnInteract() => getPickable?.OnInteract(holdPosition);

        public void OnDrop() => getPickable = null;

        public void OnSave()
        {
            // TODO: Add to inventory
            getPickable = null;
        }
    }
}