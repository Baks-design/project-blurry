using System;
using System.Collections;
using Assets.Scripts.Runtime.Components.VFX;
using Assets.Scripts.Runtime.Utilities.Helpers;
using Assets.Scripts.Runtime.Utilities.Patterns.ServicesLocator;
using KBCore.Refs;
using Unity.Cinemachine;
using UnityEngine;

namespace Assets.Scripts.Runtime.Systems.Interaction
{
    public class InteractionVFX : MonoBehaviour
    {
        [SerializeField, Anywhere] CinemachineCamera cinemachine;
        [SerializeField, Anywhere] InputReader input;

        [Header("Detection Settings")]
        [SerializeField, Range(0.1f, 1f)] float sphereCastRadius = 0.5f;

        [Header("Camera Settings")]
        [SerializeField] float changeFov = 45f;
        [SerializeField] float defaultFov = 80f;
        [Tooltip("Toggle or hold")]
        [SerializeField] bool toggle = false;
        [Tooltip("Is this script active or not")]
        [SerializeField] bool working = true;

        [Header("Depth of Field Settings")]
        [Tooltip("Quality of depth of field focus post processing blur")]
        [SerializeField] DoFAFocusQuality focusQuality = DoFAFocusQuality.NORMAL;
        public enum DoFAFocusQuality { NORMAL, HIGH }
        [Tooltip("Which layers does raycast for depth of field use?")]
        [SerializeField] LayerMask hitLayer = 1;
        [Tooltip("Max raycast distance for depth of field effect")]
        [SerializeField] float maxDistance = 100f;
        [Tooltip("Focus lerps in to target or instant")]
        [SerializeField] bool interpolateFocus = true;
        [Tooltip("for easing in of depth of field, .1 is safe")]
        [SerializeField, Range(0.01f, 1f)] float interpolationTime = 0.1f;
        [Tooltip("Is ignoring dynamic depth of field calculation")]
        [SerializeField] bool ignoreDOP = false;

        bool _isSquinting = false;
        float _currentFov;
        float _vignetteTimer = 0.2f;
        const float FOV_LERP_SPEED = 1.5f;
        const float VIGNETTE_LERP_SPEED = 0.5f;
        GameObject _doFFocusTarget;
        Vector3 _lastDoFPoint;
        Vector3 viewportToWorldPoint = new(0.5f, 0.5f, 0f);
        Camera mainCam;
        Coroutine _focusCoroutine;
        readonly RaycastHit[] _spherecastHits = new RaycastHit[1];
        IVolumeService volumeService;

        void Awake()
        {
            mainCam = Camera.main;
            cinemachine.Lens.FieldOfView = defaultFov;
            _currentFov = defaultFov;
            _doFFocusTarget = new GameObject("DoFFocusTarget") { hideFlags = HideFlags.HideInHierarchy };
        }

        void OnEnable()
        {
            input.SquintDown += OnSquintDown;
            input.SquintHold += OnSquintHold;
        }

        void OnDisable()
        {
            input.SquintDown -= OnSquintDown;
            input.SquintHold -= OnSquintHold;
        }

        void OnSquintDown()
        {
            if (toggle)
                _isSquinting = !_isSquinting;
        }

        void OnSquintHold(bool value)
        {
            if (!toggle)
                _isSquinting = value;
        }

        void Start() => ServiceLocator.Global.Get(out volumeService);
        
        void FixedUpdate()
        {
            if (focusQuality is DoFAFocusQuality.NORMAL && !ignoreDOP)
                Focus();
        }

        void Update()
        {
            if (focusQuality is DoFAFocusQuality.HIGH && !ignoreDOP)
                Focus();

            UpdateFOV();
        }

        void Focus()
        {
            var rayOrigin = mainCam.ViewportToWorldPoint(viewportToWorldPoint);
            var hitCount = Physics.SphereCastNonAlloc(
                rayOrigin,
                sphereCastRadius,
                mainCam.transform.forward,
                _spherecastHits,
                maxDistance,
                hitLayer,
                QueryTriggerInteraction.Ignore
            );
            if (hitCount == 0 || Vector3.SqrMagnitude(_lastDoFPoint - _spherecastHits[0].point) < 0.0001f)
                return;

            if (interpolateFocus)
            {
                if (_focusCoroutine != null)
                    StopCoroutine(_focusCoroutine);

                _focusCoroutine = StartCoroutine(InterpolateFocus(_spherecastHits[0].point));
                //await InterpolateFocus(_spherecastHits[0].point);
            }
            else
            {
                var focusTarget = _doFFocusTarget.transform;
                var cameraTransform = mainCam.transform;

                focusTarget.position = _spherecastHits[0].point;
                volumeService.DepthOfField.focusDistance.value = Vector3.Distance(focusTarget.position, cameraTransform.position);
            }

            _lastDoFPoint = _spherecastHits[0].point;
        }

        IEnumerator InterpolateFocus(Vector3 targetPosition)
        {
            var start = _doFFocusTarget.transform.position;
            var dTime = 0f;

            while (dTime < 1f)
            {
                yield return null;
                dTime += Time.deltaTime / interpolationTime;
                _doFFocusTarget.transform.position = Vector3.Lerp(start, targetPosition, dTime);
                volumeService.DepthOfField.focusDistance.value = 
                    Vector3.Distance(_doFFocusTarget.transform.position, mainCam.transform.position);
            }

            _doFFocusTarget.transform.position = targetPosition;
        }

        // async Awaitable InterpolateFocus(Vector3 targetPosition)
        // {
        //     var start = _doFFocusTarget.transform.position;
        //     var dTime = 0f;

        //     while (dTime < 1f)
        //     {
        //         await Awaitable.NextFrameAsync(); 
        //         dTime += Time.deltaTime / interpolationTime;
        //         _doFFocusTarget.transform.position = Vector3.Lerp(start, targetPosition, dTime);
        //         volumeService.DepthOfField.focusDistance.value = Vector3.Distance(_doFFocusTarget.transform.position, tr.position);
        //     }

        //     _doFFocusTarget.transform.position = targetPosition;
        // }

        void UpdateFOV()
        {
            HandleFOV();
            UpdateVignette();
        }

        void HandleFOV()
        {
            if (!working)
            {
                _currentFov = Mathf.Lerp(_currentFov, defaultFov, Time.deltaTime * FOV_LERP_SPEED);
                volumeService.Vignette.intensity.value = 0.2f;
                return;
            }

            var targetFov = _isSquinting ? changeFov : defaultFov;
            _currentFov = Mathf.Lerp(_currentFov, targetFov, Time.deltaTime * FOV_LERP_SPEED);

            cinemachine.Lens.FieldOfView = _currentFov;
        }

        void UpdateVignette()
        {
            _vignetteTimer += Time.deltaTime * VIGNETTE_LERP_SPEED * (_isSquinting ? 1f : -1f);
            _vignetteTimer = Mathf.Clamp(_vignetteTimer, 0.1f, 0.4f);
            volumeService.Vignette.intensity.value = _vignetteTimer;
        }

        void OnDestroy()
        {
            if (_doFFocusTarget != null)
                Destroy(_doFFocusTarget);
        }
    }
}