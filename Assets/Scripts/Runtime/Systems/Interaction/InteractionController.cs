using System;
using UnityEngine;
using KBCore.Refs;
using Assets.Scripts.Runtime.Components.VFX;
using Assets.Scripts.Runtime.Systems.EventBus;
using Assets.Scripts.Runtime.Systems.EventBus.Events;
using Assets.Scripts.Runtime.Systems.Interaction.States;
using Assets.Scripts.Runtime.Utilities.Helpers;
using Assets.Scripts.Runtime.Utilities.Patterns.ServicesLocator;
using Assets.Scripts.Runtime.Utilities.Patterns.StateMachine;

namespace Assets.Scripts.Runtime.Systems.Interaction
{
    public class InteractionController : StatefulEntity
    {
        [SerializeField, Self] Transform tr;
        [SerializeField, Anywhere] InputReader inputReader;
        [SerializeField] LayerMask interactableLayer;
        [SerializeField, Range(0.5f, 5f)] float rayDistance = 2f;
        [SerializeField, Range(0.1f, 1f)] float raySphereRadius = 0.5f;
        bool pickUpInput;
        bool dropInput;
        Vector2 lookInput;
        Transform cameraTransform;
        Transform getSomething;
        RaycastHit[] hits;
        IPickable getPickable;
        IVolumeService volumeService;

        protected override void Awake()
        {
            base.Awake();
            SetupStateMachine();
        }

        void SetupStateMachine()
        {
            var search = new SearchState(this);
            var pickUp = new PickUpState(this);
            var drop = new DropState(this);
            var save = new SaveState(this);

            At(drop, search, getPickable == null);
            At(save, search, getPickable == null);
            At(search, pickUp, pickUpInput && getPickable != null);
            At(pickUp, save, pickUpInput && getPickable != null);
            At(pickUp, drop, dropInput && getPickable != null);

            stateMachine.SetState(search);
        }

        void Start()
        {
            GetServices();
            InitVars();
        }

        void GetServices() => ServiceLocator.Global.Get(out volumeService);

        void InitVars()
        {
            pickUpInput = false;
            dropInput = false;
            lookInput = Vector2.zero;
            cameraTransform = Camera.main.transform;
            getSomething = null;
            hits = new RaycastHit[1];
        }

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

        void LookInput(Vector2 input, bool _) => lookInput = input;

        void PickupInput(bool input)
        {
            pickUpInput = input;
            
            if (getSomething != null)
                getSomething.TryGetComponent(out getPickable);
        }

        void DropInput(bool input) => dropInput = input;

        public void OnCheck()
        {
            var hitCount = Physics.SphereCastNonAlloc(
                cameraTransform.position, raySphereRadius, cameraTransform.forward,
                hits, rayDistance, interactableLayer, QueryTriggerInteraction.Ignore);

            var lastHitSomething = hitCount > 0;
            getSomething = lastHitSomething ? hits[0].transform : null;
        }

        public void OnPickUp()
        {
            volumeService.ToggleDepthOfField(true);
            getPickable.OnPickUp(cameraTransform.position, cameraTransform.rotation, lookInput);
        }

        public void OnDrop()
        {
            volumeService.ToggleDepthOfField(false);
            getPickable.OnDrop(tr.position);
            getPickable = null;
        }

        public void OnSave()
        {
            volumeService.ToggleDepthOfField(false);
            getPickable.OnSave(tr.position);
            getPickable = null;
            // TODO: Add to inventory
        }
    }
}