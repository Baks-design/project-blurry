using KBCore.Refs;
using UnityEngine;

public class TorchAnimator : MonoBehaviour
{
    [SerializeField, Self] Transform _transform;
    [Tooltip("Amount of variation in each axis (X, Y, Z).")]
    [SerializeField] Vector3 Amount = new(1f, 1f, 0f);
    [Tooltip("Duration of the variation cycle.")]
    [SerializeField] float Duration = 1f;
    [Tooltip("Speed of the variation.")]
    [SerializeField] float Speed = 1f;
    [Tooltip("Fade in/out curve for smooth transitions.")]
    [SerializeField] AnimationCurve Curve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
    float _time;

    void LateUpdate()
    {
        if (_time > 0f)
        {
            _time -= Time.deltaTime;
            if (_time > 0f)
                AnimateTorch();
            else
                ResetAnimation();
        }
        else
            _time = Duration;
    }

    void AnimateTorch()
    {
        var curveValue = Curve.Evaluate(1f - _time / Duration);
        var nextPos = CalculatePerlinNoiseOffset(curveValue);
        _transform.localPosition = nextPos;
    }

    Vector3 CalculatePerlinNoiseOffset(float curveValue)
    {
        var noiseX = (Mathf.PerlinNoise(_time * Speed, _time * Speed * 2f) - 0.5f) * Amount.x;
        var noiseY = (Mathf.PerlinNoise(_time * Speed * 2f, _time * Speed) - 0.5f) * Amount.y;
        var noiseZ = (Mathf.PerlinNoise(_time * Speed, _time * Speed * 3f) - 0.5f) * Amount.z;
        return curveValue * noiseX * _transform.right +
               curveValue * noiseY * _transform.up +
               curveValue * noiseZ * _transform.forward;
    }

    void ResetAnimation() => _transform.localPosition = Vector3.zero;
}
