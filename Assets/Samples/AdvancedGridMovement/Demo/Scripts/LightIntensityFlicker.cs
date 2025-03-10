using KBCore.Refs;
using UnityEngine;

public class LightIntensityFlicker : MonoBehaviour
{
    [SerializeField, Self] Light _lightSource;
    [SerializeField, Range(0.1f, 2f)] float minimumIntensity = 0.75f;
    [SerializeField, Range(0.1f, 2f)] float maximumIntensity = 1.5f;
    [SerializeField, Range(0.1f, 1f)] float flickerDuration = 0.25f;
    float _elapsedTime, _timeScale, _lastIntensity, _targetIntensity;

    void Start() => InitializeFlicker();

    void InitializeFlicker()
    {
        _timeScale = 1f / flickerDuration;
        _lastIntensity = _lightSource.intensity = minimumIntensity;
        _targetIntensity = maximumIntensity;
    }

    void Update()
    {
        _elapsedTime += Time.deltaTime;

        if (_elapsedTime > flickerDuration)
        {
            UpdateTargetIntensity();
            _elapsedTime = 0f;
        }

        _lightSource.intensity = Mathf.Lerp(_lastIntensity, _targetIntensity, _elapsedTime * _timeScale);
    }

    void UpdateTargetIntensity()
    {
        _lastIntensity = _targetIntensity;
        _targetIntensity = Random.Range(minimumIntensity, maximumIntensity);
    }
}