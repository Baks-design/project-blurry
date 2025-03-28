using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Assets.Samples.Fog.Runtime
{
    public enum FullScreenFogMode { Depth, Distance, Height }

    [Serializable]
    public sealed class FullScreenFogModeParameter : VolumeParameter<FullScreenFogMode>
    {
        public FullScreenFogModeParameter(FullScreenFogMode value, bool overrideState = false) : base(value, overrideState) { }
    }

    public enum FullScreenFogDensityMode { Linear, Exponential, ExponentialSquared }

    [Serializable]
    public sealed class FullScreenFogDensityModeParameter : VolumeParameter<FullScreenFogDensityMode>
    {
        public FullScreenFogDensityModeParameter(FullScreenFogDensityMode value, bool overrideState = false) : base(value, overrideState) { }
    }

    public enum FullScreenFogNoiseMode { Off, Procedural, Texture }

    [Serializable]
    public sealed class FullScreenFogNoiseModeParameter : VolumeParameter<FullScreenFogNoiseMode>
    {
        public FullScreenFogNoiseModeParameter(FullScreenFogNoiseMode value, bool overrideState = false) : base(value, overrideState) { }
    }

    [Serializable, VolumeComponentMenu("Baks/Full Screen Fog")]
    [SupportedOnRenderPipeline(typeof(UniversalRenderPipelineAsset))]
    public class FullScreenFog : VolumeComponent, IPostProcessComponent
    {
        public const string Name = "Full Screen Fog";

        [Header("Fog")]
        [Tooltip("Calculation mode of the fog.")]
        public FullScreenFogModeParameter mode = new(FullScreenFogMode.Distance);
        [Tooltip("Amount of the fog.")]
        public ClampedFloatParameter intensity = new(0f, 0f, 1f);
        [Tooltip("Fog color.")]
        public ColorParameter color = new(Color.gray, true, false, true);
        [Tooltip("Density mode of the fog.")]
        public FullScreenFogDensityModeParameter densityMode = new(FullScreenFogDensityMode.ExponentialSquared);
        [Tooltip("Start depth or distance.")]
        public FloatParameter startLine = new(0f);
        [Tooltip("End depth or distance.")]
        public FloatParameter endLine = new(10f);
        [Tooltip("Start height.")]
        public FloatParameter startHeight = new(5f);
        [Tooltip("End height.")]
        public FloatParameter endHeight = new(0f);
        [Tooltip("Factor of the density mode.")]
        public ClampedFloatParameter density = new(0.1f, 0f, 1f);

        [Header("Noise")]
        [Tooltip("Noise mode for the fog.")]
        public FullScreenFogNoiseModeParameter noiseMode = new(FullScreenFogNoiseMode.Off);
        [Tooltip("Texture used by the noise.")]
        public TextureParameter noiseTexture = new(null);
        [Tooltip("Mixing strength of the noise.")]
        public ClampedFloatParameter noiseIntensity = new(0.5f, 0f, 1f);
        [Tooltip("Scaling of the noise.")]
        public MinFloatParameter noiseScale = new(1f, 0f);
        [Tooltip("Scrolling speed of the noise.")]
        public Vector2Parameter noiseScrollSpeed = new(Vector2.one);

        public static bool UseStartLine(FullScreenFogMode mode) => mode == FullScreenFogMode.Depth || mode == FullScreenFogMode.Distance;
        public static bool UseEndLine(FullScreenFogMode mode, FullScreenFogDensityMode densityMode)
        => UseStartLine(mode) && densityMode == FullScreenFogDensityMode.Linear;
        public static bool UseStartHeight(FullScreenFogMode mode) => mode == FullScreenFogMode.Height;
        public static bool UseEndHeight(FullScreenFogMode mode, FullScreenFogDensityMode densityMode)
        => UseStartHeight(mode) && densityMode == FullScreenFogDensityMode.Linear;
        public static bool UseIntensity(FullScreenFogDensityMode mode)
        => mode == FullScreenFogDensityMode.Exponential || mode == FullScreenFogDensityMode.ExponentialSquared;
        public static bool UseNoiseTex(FullScreenFogNoiseMode noiseMode) => noiseMode == FullScreenFogNoiseMode.Texture;
        public static bool UseNoiseIntensity(FullScreenFogNoiseMode noiseMode) => noiseMode != FullScreenFogNoiseMode.Off;
        public bool IsActive() => intensity.value != 0;
        public bool IsTileCompatible() => true;
    }
}