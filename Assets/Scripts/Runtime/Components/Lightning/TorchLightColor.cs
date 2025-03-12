using UnityEngine;
using LitMotion;
using KBCore.Refs;

namespace Assets.Scripts.Runtime.Components.Lightning
{
    public class TorchLightColor : MonoBehaviour
    {
        [SerializeField, Child] Light torchLight;
        [SerializeField] Color startColor = Color.yellow;
        [SerializeField] Color endColor = Color.red;
        [SerializeField] float minIntensity = 1f;
        [SerializeField] float maxIntensity = 3f;
        [SerializeField] float duration = 1f;

        void Start()
        {
            LMotion.Create(minIntensity, maxIntensity, duration)
                .WithLoops(-1, LoopType.Yoyo) 
                .Bind(x => torchLight.intensity = x);

            LMotion.Create(startColor, endColor, duration)
                .WithLoops(-1, LoopType.Yoyo) 
                .Bind(x => torchLight.color = x); 
        }
    }
}
