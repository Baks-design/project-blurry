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

        void Awake()
        {
            DontDestroyOnLoad(this);
            ServiceLocator.Global.Register<IVolumeService>(this);
        }

        public void ToggleDepthOfField(bool enable)
        {
            if (globalVolume.profile.TryGet(out DepthOfField depthOfField))
                depthOfField.active = enable;
            else
                Assert.IsNull(depthOfField, "DepthOfField effect not found in the Volume profile.");
        }
    }
}