using UnityEngine;
using LitMotion;
using KBCore.Refs;

namespace Assets.Scripts.Runtime.Components.VFX
{
    public class TorchLightFlicker : MonoBehaviour
    {
        [SerializeField, Child] Light _lightSource;
        [SerializeField, Range(0.1f, 2f)] float minimumIntensity = 0.75f;
        [SerializeField, Range(0.1f, 2f)] float maximumIntensity = 1.5f;
        [SerializeField, Range(0.1f, 1f)] float flickerDuration = 0.25f;

        void Start() => FlickerIntensity();

        void FlickerIntensity()
        {
            var targetIntensity = Random.Range(minimumIntensity, maximumIntensity);

            LMotion.Create(_lightSource.intensity, targetIntensity, flickerDuration)
                .WithEase(Ease.OutQuad)
                .WithLoops(-1, LoopType.Restart)
                .WithOnComplete(() => FlickerIntensity())
                .Bind(x => _lightSource.intensity = x);
        }
    }
}