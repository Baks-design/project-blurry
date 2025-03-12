using System;
using Assets.Scripts.Runtime.Entities.Player;
using UnityEngine;

namespace Assets.Scripts.Runtime.Systems.Interaction
{
    public class InteractionController : MonoBehaviour
    {
        [SerializeField] InputReader inputReader;
        [SerializeField] LayerMask interactableLayer;
        [SerializeField, Range(0.5f, 5f)] float rayDistance = 2f;
        [SerializeField, Range(0.1f, 1f)] float raySphereRadius = 0.5f;
        bool lastHitSomething;
        Ray lastRay;
        Transform hitTransform;
        Transform cameraTransform;
        IPickable pickable;
        readonly RaycastHit[] hits = new RaycastHit[1];

        void Start() => cameraTransform = Camera.main.transform;

        void OnEnable()
        {
            inputReader.Interaction += PickUp;
            inputReader.Interaction += AddToInventory;
        }

        void OnDisable()
        {
            inputReader.Interaction -= PickUp;
            inputReader.Interaction -= AddToInventory;
        }

        void PickUp()
        {
            if (!pickable.IsPicked && hitTransform.TryGetComponent(out pickable))
                pickable.OnPickUp();
        }

        void AddToInventory()
        {
            if (pickable.IsPicked)
            {
                //TODO: add to inv
            }
        }

        void Update()
        {
            CheckForInteractable();
            ManipulateItem();
        }

        void CheckForInteractable()
        {
            lastRay = new Ray(cameraTransform.position, cameraTransform.forward);
            var hitCount = Physics.SphereCastNonAlloc(
                lastRay, raySphereRadius, hits, rayDistance, interactableLayer, QueryTriggerInteraction.Ignore);
            lastHitSomething = hitCount > 0;
            hitTransform = lastHitSomething ? hits[0].transform : null;
        }

        void ManipulateItem()
        {
            if (pickable != null && pickable.IsPicked)
                pickable.OnManipulate();
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