using System;
using Assets.Scripts.Runtime.Entities.Player;
using Assets.Scripts.Runtime.Systems.EventBus;
using Assets.Scripts.Runtime.Systems.EventBus.Events;
using KBCore.Refs;
using UnityEngine;

namespace Assets.Scripts.Runtime.Systems.Interaction
{
    public class InteractionController : MonoBehaviour
    {
        [SerializeField, Anywhere] Transform tr;
        [SerializeField, Anywhere] VolumeController volumeController;
        [SerializeField, Anywhere] InputReader inputReader;
        [SerializeField] LayerMask interactableLayer;
        [SerializeField, Range(0.5f, 5f)] float rayDistance = 2f;
        [SerializeField, Range(0.1f, 1f)] float raySphereRadius = 0.5f;
        bool lastHitSomething, isHoldingItem;
        Ray lastRay;
        Vector2 look;
        Transform hitTransform, cameraTransform;
        readonly RaycastHit[] hits = new RaycastHit[1];
        IPickable pickable;

        void Start() => cameraTransform = Camera.main.transform;

        void OnEnable()
        {
            inputReader.Look += LookInput;
            inputReader.Interaction += HandleInteraction;
        }

        void OnDisable()
        {
            inputReader.Look -= LookInput;
            inputReader.Interaction -= HandleInteraction;
        }

        void LookInput(Vector2 value, bool isC) => look = value;

        void HandleInteraction()
        {
            if (isHoldingItem)
                AddInventory();
            else
                PickUp();
        }

        void PickUp()
        {
            if (hitTransform != null && hitTransform.TryGetComponent(out pickable) && !pickable.IsPicked)
            {
                isHoldingItem = true;
                EventBus<PickItemEvent>.Raise(new PickItemEvent { isMoveValid = false });
                volumeController.ToggleDepthOfField(true);
            }
        }

        void AddInventory()
        {
            if (pickable != null && pickable.IsPicked)
            {
                isHoldingItem = false;
                EventBus<PickItemEvent>.Raise(new PickItemEvent { isMoveValid = true });
                pickable = null;
                volumeController.ToggleDepthOfField(false);
                //TODO: Add to inventory;
            }
        }

        void Update()
        {
            CheckForInteractable();
            HandleObjects();
        }

        void CheckForInteractable()
        {
            lastRay = new Ray(cameraTransform.localPosition, cameraTransform.forward);
            var hitCount = Physics.SphereCastNonAlloc(
                lastRay, raySphereRadius, hits, rayDistance, interactableLayer, QueryTriggerInteraction.Ignore);
            lastHitSomething = hitCount > 0;
            hitTransform = lastHitSomething ? hits[0].transform : null;
        }

        void HandleObjects() //FIXME: Handle
        {
            if (!isHoldingItem)
                pickable.OnPickUp(hitTransform.localPosition, cameraTransform.localRotation);
            else if (pickable != null && pickable.IsPicked)
                pickable.OnManipulate(look);
            else if (isHoldingItem)
                pickable.OnAddInventory(tr.localPosition);
        }

        void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
            Gizmos.color = lastHitSomething ? Color.red : Color.green;
            Gizmos.DrawLine(lastRay.origin, lastRay.origin + lastRay.direction * rayDistance);
            Gizmos.DrawWireSphere(lastRay.origin + lastRay.direction * rayDistance, raySphereRadius);
        }
    }
}