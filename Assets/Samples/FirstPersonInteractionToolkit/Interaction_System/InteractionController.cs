using System;
using Game.Runtime.Components.Inputs;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    [SerializeField] InputReader inputReader;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField, Range(0.5f, 5f)] float rayDistance = 2f;
    [SerializeField, Range(0.1f, 1f)] float raySphereRadius = 0.5f;
    bool _lastHitSomething;
    Ray _lastRay;
    Transform _hitTransform, _cameraTransform;
    readonly RaycastHit[] _hits = new RaycastHit[1];

    void Start() => _cameraTransform = Camera.main.transform;

    void OnEnable() => inputReader.OnInteraction += CheckForInteractable;

    void OnDisable() => inputReader.OnInteraction -= CheckForInteractable;

    void CheckForInteractable()
    {
        _lastRay = new Ray(_cameraTransform.position, _cameraTransform.forward);
        var hitCount = Physics.SphereCastNonAlloc(_lastRay, raySphereRadius, _hits, rayDistance, interactableLayer);
        _lastHitSomething = hitCount > 0;
        _hitTransform = _lastHitSomething ? _hits[0].transform : null;

        PerformActions();
    }

    void PerformActions()
    {
        if (_lastHitSomething)
        {
            PerformAction<IPickable>(pickable => pickable.OnPickUp());
            PerformAction<IPickable>(pickable => pickable.OnHold());
            PerformAction<IPickable>(pickable => pickable.OnRelease());
        }
    }

    void PerformAction<T>(Action<T> action) where T : class
    {
        if (_hitTransform.TryGetComponent(out T component))
            action(component);
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        Gizmos.color = _lastHitSomething ? Color.red : Color.green;
        Gizmos.DrawLine(_lastRay.origin, _lastRay.origin + _lastRay.direction * rayDistance);
        Gizmos.DrawWireSphere(_lastRay.origin + _lastRay.direction * rayDistance, raySphereRadius);
    }
}