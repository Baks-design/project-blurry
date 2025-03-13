using KBCore.Refs;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Assets.Scripts.Runtime.Systems.Interaction
{
    public class VolumeController : MonoBehaviour
    {
        [SerializeField, Child] Volume globalVolume;

        public void ToggleDepthOfField(bool enable)
        {
            if (globalVolume.profile.TryGet(out DepthOfField depthOfField))
                depthOfField.active = enable;
            else
                Debug.LogWarning("DepthOfField effect not found in the Volume profile.");
        }
    }
}