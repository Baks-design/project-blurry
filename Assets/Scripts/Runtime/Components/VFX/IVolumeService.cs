using UnityEngine.Rendering.Universal;

namespace Assets.Scripts.Runtime.Components.VFX
{
    public interface IVolumeService
    {
        DepthOfField DepthOfField { get; }
        Vignette Vignette { get; }
    }
}