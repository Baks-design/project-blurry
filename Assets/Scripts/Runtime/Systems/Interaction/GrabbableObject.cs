using System.Collections;
using Assets.Scripts.Runtime.Systems.Audio;
using Assets.Scripts.Runtime.Systems.EventBus;
using Assets.Scripts.Runtime.Systems.EventBus.Events;
using Assets.Scripts.Runtime.Utilities.Helpers;
using Assets.Scripts.Runtime.Utilities.Patterns.ServicesLocator;
using KBCore.Refs;
using UnityEngine;

namespace Assets.Scripts.Runtime.Systems.Interaction
{
    [RequireComponent(typeof(Rigidbody))]
    public class GrabbableObject : MonoBehaviour, IPickable, IHoverable
    {
        [Header("Object Settings")]
        [SerializeField, Self] Transform tr;
        [SerializeField, Self] Rigidbody rb;
        [Tooltip("Primary renderer of this object")]
        [SerializeField, Self] Renderer rend;
        [Tooltip("Color when hovered over (looked at)")]
        [SerializeField] Color targetColor = Color.yellow;
        [Tooltip("If object is returned to original position, snaps into place")]
        [SerializeField] bool canReturn = true;

        [Header("Interaction Settings")]
        [SerializeField, Anywhere] InputReader input;
        [Tooltip("Rotation Sensitivity")]
        [SerializeField] float sensitivity = 5f;
        [Tooltip("200 Strong, 50 weak")]
        [SerializeField] float throwSpeed = 200f;

        [Header("Audio Settings")]
        [SerializeField] float velocityClipSplit = 5f;
        [SerializeField] SoundData pickupData;
        [SerializeField] SoundData throwData;
        [SerializeField] SoundData crashsoftData;
        [SerializeField] SoundData crashhardData;

        [Header("UI Settings")]
        [SerializeField] string hoverPrompt = "Grab Object";
        [SerializeField] string interactPrompt = "";

        bool isHovered;
        bool isCarried;
        bool isRotating;
        bool objectReset = true;
        bool muteCollisionSound = true;
        bool lookInteractionDown;
        bool lookInteractionHold;
        Vector2 look;
        Vector3 originPosition;
        Transform mainCameraTransform;
        Quaternion currentRotation;
        Quaternion originRotation;
        Color originColor;
        SoundBuilder _soundBuilder;
        ISoundService soundService;

        void Awake()
        {
            originColor = rend.material.color;
            mainCameraTransform = Camera.main.transform;
            ServiceLocator.Global.Get(out soundService);
            _soundBuilder = soundService.CreateSoundBuilder();
        }

        void OnEnable()
        {
            input.Look += OnLook;
            input.LookInteractionDown += OnLookInteractionDown;
            input.LookInteractionHold += OnLookInteractionHold;
        }

        void OnDisable()
        {
            input.Look -= OnLook;
            input.LookInteractionDown -= OnLookInteractionDown;
            input.LookInteractionHold -= OnLookInteractionHold;
        }

        void OnLook(Vector2 value, bool _) => look = value;

        void OnLookInteractionDown(bool isValue) => lookInteractionDown = isValue;

        void OnLookInteractionHold(bool isValue) => lookInteractionHold = isValue;

        void Start() => StartCoroutine(SetOriginTransform());

        IEnumerator SetOriginTransform()
        {
            yield return WaitFor.Seconds(0.25f);
            originPosition = tr.position;
            originRotation = tr.rotation;
            yield return WaitFor.Seconds(0.75f);
            muteCollisionSound = false;
        }

        void FixedUpdate() => HandleObjectHoverEffect();

        void HandleObjectHoverEffect()
        {
            var orColor = Color.Lerp(rend.material.color, originColor, Time.deltaTime * 2f);
            var tarColor = Color.Lerp(rend.material.color, targetColor, Time.deltaTime * 4f);
            rend.material.color = !isCarried ? (isHovered ? tarColor : orColor) : originColor;
        }

        void Update()
        {
            if (isCarried)
                HandleCarriedObjectInput();
            else if (canReturn)
                CheckForReturnToOrigin();
        }

        void HandleCarriedObjectInput()
        {
            if (lookInteractionDown && !isRotating)
            {
                EventBus<CameraMovementEvent>.Raise(new CameraMovementEvent { isLookValid = false });
                isRotating = true;
            }

            if (lookInteractionHold)
                CalculateRelativeRotation();

            if (!lookInteractionHold && isRotating)
            {
                EventBus<CameraMovementEvent>.Raise(new CameraMovementEvent { isLookValid = true });
                isRotating = false;
            }
        }

        void CalculateRelativeRotation()
        {
            var relativeUp = mainCameraTransform.TransformDirection(Vector3.up);
            var relativeRight = mainCameraTransform.TransformDirection(Vector3.right);

            var objectRelativeUp = tr.InverseTransformDirection(relativeUp);
            var objectRelativeRight = tr.InverseTransformDirection(relativeRight);

            var rotateLeft = Quaternion.AngleAxis(-look.x * sensitivity / tr.localScale.x * sensitivity, objectRelativeUp);
            var rotateRight = Quaternion.AngleAxis(look.y * sensitivity / tr.localScale.x * sensitivity, objectRelativeRight);

            currentRotation = rotateLeft * rotateRight;

            if (isRotating)
                tr.localRotation *= currentRotation;
        }

        void CheckForReturnToOrigin()
        {
            if (objectReset || Vector3.Distance(originPosition, tr.position) > 0.25f) return;

            rb.isKinematic = true;
            tr.SetPositionAndRotation(originPosition, originRotation);
            rb.isKinematic = false;
            objectReset = true;
        }

        void OnTriggerEnter(Collider other)
        {
            if (isCarried) return;

            HandleCollisionEffects();
        }

        void HandleCollisionEffects()
        {
            if (muteCollisionSound) return;

            var clipIndex = rb.linearVelocity.magnitude < velocityClipSplit ? crashsoftData : crashhardData;
            _soundBuilder.WithPosition(tr.position).Play(clipIndex);
        }

        #region Hover
        public void OnHoverStart()
        {
            isHovered = true;

            StartCoroutine(ResetHoverAfterDelay());

            if (!isCarried)
                EventBus<InteractObjectNameEvent>.Raise(new InteractObjectNameEvent { objectName = hoverPrompt });
        }

        IEnumerator ResetHoverAfterDelay()
        {
            yield return WaitFor.Seconds(1f);
            isHovered = false;
        }
        #endregion

        #region Interaction
        public void OnInteract(Transform holdPosition)
        {
            isCarried = !isCarried;

            if (isCarried)
                PickUpObject(holdPosition);
            else
                ThrowObject();

            EventBus<InteractObjectNameEvent>.Raise(new InteractObjectNameEvent { objectName = interactPrompt });
            EventBus<InteractCrosshairEvent>.Raise(new InteractCrosshairEvent { toggleCrosshair = false });
        }

        void PickUpObject(Transform holdPosition)
        {
            rb.isKinematic = true;
            tr.position = holdPosition.position;
            tr.parent = mainCameraTransform;

            _soundBuilder.WithPosition(tr.position).Play(pickupData);
            objectReset = false;
        }

        void ThrowObject()
        {
            rb.isKinematic = false;
            tr.parent = null;
            rb.AddForce(mainCameraTransform.forward * throwSpeed);

            _soundBuilder.WithPosition(tr.position).Play(throwData);

            isRotating = false;

            EventBus<CameraMovementEvent>.Raise(new CameraMovementEvent { isLookValid = true });
            EventBus<InteractCrosshairEvent>.Raise(new InteractCrosshairEvent { toggleCrosshair = true });
        }
        #endregion
    }
}