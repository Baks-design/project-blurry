using UnityEngine;

[CreateAssetMenu]
public class AnimationCurveAsset : ScriptableObject
{
    public AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    public static implicit operator AnimationCurve(AnimationCurveAsset me) => me.curve;

    public static implicit operator AnimationCurveAsset(AnimationCurve curve)
    {
        var asset = CreateInstance<AnimationCurveAsset>();
        asset.curve = curve;
        return asset;
    }
}
