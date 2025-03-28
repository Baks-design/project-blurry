using Assets.Scripts.Runtime.Utilities.Patterns.ServicesLocator;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Assets.Scripts.Runtime.Components.VFX
{
    public class VolumeService : MonoBehaviour, IVolumeService
    {
        [SerializeField, Child] Volume globalVolume;
        DepthOfField depthOfField;
        Vignette vignette;

        public DepthOfField DepthOfField => depthOfField;
        public Vignette Vignette => vignette;

        void Awake()
        {
            DontDestroyOnLoad(this);

            ServiceLocator.Global.Register<IVolumeService>(this);

            globalVolume.profile.TryGet(out depthOfField);
            globalVolume.profile.TryGet(out vignette);
        }
    }
}