#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

//------------------------------------------------------------------------------------------------------
// Global Multi-Compile Pragmas
//------------------------------------------------------------------------------------------------------

#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_SCREEN
#pragma multi_compile _ _SHADOWS_SOFT
#pragma multi_compile _ _ADDITIONAL_LIGHTS
#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
#pragma multi_compile _ _LIGHT_LAYERS
#pragma multi_compile _ _LIGHT_COOKIES
#pragma multi_compile _ _FORWARD_PLUS

//------------------------------------------------------------------------------------------------------
// Main Light
//------------------------------------------------------------------------------------------------------

/*
- Obtains the Direction, Color and Distance Atten for the Main Light.
- (DistanceAtten is either 0 or 1 for directional light, depending if the light is in the culling mask or not)
- If you want shadow attenutation, see MainLightShadows_float, or use MainLightFull_float instead
*/
void MainLight_float(out float3 Direction, out float3 Color, out float DistanceAtten)
{
    #ifdef SHADERGRAPH_PREVIEW
        Direction = normalize(float3(1, 1, -0.4)); // Default direction for preview
        Color = float3(1, 1, 1); // Default color for preview
        DistanceAtten = 1; // Default attenuation for preview
    #else
        Light mainLight = GetMainLight(); // Get the main light
        Direction = mainLight.direction;
        Color = mainLight.color;
        DistanceAtten = mainLight.distanceAttenuation;
    #endif
}

//------------------------------------------------------------------------------------------------------
// Main Light Layer Test
//------------------------------------------------------------------------------------------------------
		
/*
- Tests whether the Main Light Layer Mask appears in the Rendering Layers from renderer
- (Used to support Light Layers, pass your shading from Main Light into this)
*/
void MainLightLayer_float(float3 Shading, out float3 Out)
{
    #ifdef SHADERGRAPH_PREVIEW
        Out = Shading; // Default output for preview
    #else
        Out = 0; // Default output if light layer doesn't match
        uint meshRenderingLayers = GetMeshRenderingLayer(); // Get mesh rendering layers
        #ifdef _LIGHT_LAYERS
            if (IsMatchingLightLayer(GetMainLight().layerMask, meshRenderingLayers))
        #endif
        {
            Out = Shading; // Apply shading if light layer matches
        }
    #endif
}

/*
- Obtains the Light Cookie assigned to the Main Light
- (For usage, You'd want to Multiply the result with your Light Colour)
*/
void MainLightCookie_float(float3 WorldPos, out float3 Cookie)
{
    Cookie = 1; // Default cookie value
    #if defined(_LIGHT_COOKIES)
        Cookie = SampleMainLightCookie(WorldPos); // Sample the main light cookie
    #endif
}

//------------------------------------------------------------------------------------------------------
// Main Light Shadows
//------------------------------------------------------------------------------------------------------

/*
- This undef (un-define) is required to prevent the "invalid subscript 'shadowCoord'" error,
  which occurs when _MAIN_LIGHT_SHADOWS is used with 1/No Shadow Cascades with the Unlit Graph.
- It's not required for the PBR/Lit graph, so I'm using the SHADERPASS_FORWARD to ignore it for that pass
*/
#ifndef SHADERGRAPH_PREVIEW
	#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
	#if (SHADERPASS != SHADERPASS_FORWARD)
		#undef REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
	#endif
#endif

/*
- Samples the Shadowmap for the Main Light, based on the World Position passed in. (Position node)
*/
void MainLightShadows_float(float3 WorldPos, half4 Shadowmask, out float ShadowAtten)
{
    #ifdef SHADERGRAPH_PREVIEW
        ShadowAtten = 1; // Default to no shadow in preview
    #else
        // Compute shadow coordinates based on the rendering path
        float4 shadowCoord;
        #if defined(_MAIN_LIGHT_SHADOWS_SCREEN) && !defined(_SURFACE_TYPE_TRANSPARENT)
            shadowCoord = ComputeScreenPos(TransformWorldToHClip(WorldPos)); // Screen-space shadows
        #else
            shadowCoord = TransformWorldToShadowCoord(WorldPos); // Traditional shadow maps
        #endif

        // Calculate shadow attenuation
        ShadowAtten = MainLightShadow(shadowCoord, WorldPos, Shadowmask, _MainLightOcclusionProbes);
    #endif
}

void MainLightShadows_float(float3 WorldPos, out float ShadowAtten)
{
    // Call the main function with a default Shadowmask value
    MainLightShadows_float(WorldPos, half4(1, 1, 1, 1), ShadowAtten);
}

//------------------------------------------------------------------------------------------------------
// Shadowmask (v10+)
//------------------------------------------------------------------------------------------------------

/*
- Used to support "Shadowmask" mode in Lighting window.
- Should be sampled once in graph, then input into the Main Light Shadows and/or Additional Light subgraphs/functions.
*/
void Shadowmask_half(float2 lightmapUV, out half4 Shadowmask)
{
    #ifdef SHADERGRAPH_PREVIEW
        Shadowmask = half4(1, 1, 1, 1); // Default shadowmask for preview
    #else
        OUTPUT_LIGHTMAP_UV(lightmapUV, unity_LightmapST, lightmapUV); // Transform lightmap UVs
        Shadowmask = SAMPLE_SHADOWMASK(lightmapUV); // Sample shadowmask
    #endif
}

//------------------------------------------------------------------------------------------------------
// Ambient Lighting
//------------------------------------------------------------------------------------------------------

/*
- Uses "SampleSH", the spherical harmonic stuff that ambient lighting / light probes uses.
- Will likely be used in the fragment, so will be per-pixel.
- Alternatively could use the Baked GI node, as it'll also handle this for you.
- Could also use the Ambient node, would be cheaper but the result won't automatically adapt based on the Environmental Lighting Source (Lighting tab).
*/
void AmbientSampleSH_float(float3 WorldNormal, out float3 Ambient)
{
    #ifdef SHADERGRAPH_PREVIEW
        Ambient = float3(0.1, 0.1, 0.1); // Default ambient for preview
    #else
        Ambient = SampleSH(WorldNormal); // Sample spherical harmonics
    #endif
}

//------------------------------------------------------------------------------------------------------
// Subtractive Baked GI
//------------------------------------------------------------------------------------------------------
/*
- Used to support "Subtractive" mode in Lighting window.
*/
void SubtractiveGI_float(float ShadowAtten, float3 normalWS, float3 bakedGI, out half3 result)
{
    #ifdef SHADERGRAPH_PREVIEW
        result = half3(1, 1, 1); // Default white for preview
    #else
        Light mainLight = GetMainLight();
        mainLight.shadowAttenuation = ShadowAtten; // Apply shadow attenuation
        MixRealtimeAndBakedGI(mainLight, normalWS, bakedGI); // Blend real-time and baked GI
        result = bakedGI; // Output the result
    #endif
}

//------------------------------------------------------------------------------------------------------
// Default Additional Lights
//------------------------------------------------------------------------------------------------------

/*
- Handles additional lights (e.g. additional directional, point, spotlights)
- For custom lighting, you may want to duplicate this and swap the LightingLambert / LightingSpecular functions out. See Toon Example below!
*/
void AdditionalLights_float(
    float3 SpecColor, float Smoothness, float3 WorldPosition, float3 WorldNormal, float3 WorldView, half4 Shadowmask,
    out float3 Diffuse, out float3 Specular)
{
    float3 diffuseColor = 0;
    float3 specularColor = 0;

#ifndef SHADERGRAPH_PREVIEW
    // Precompute Smoothness exponentiation
    Smoothness = exp2(10 * Smoothness + 1);

    // Get light count and rendering layers
    uint pixelLightCount = GetAdditionalLightsCount();
    uint meshRenderingLayers = GetMeshRenderingLayer();

    // Precompute common values
    float4 screenPos = ComputeScreenPos(TransformWorldToHClip(WorldPosition));
    float2 normalizedScreenSpaceUV = screenPos.xy / screenPos.w;

    #if USE_FORWARD_PLUS
    for (uint lightIndex = 0; lightIndex < min(URP_FP_DIRECTIONAL_LIGHTS_COUNT, MAX_VISIBLE_LIGHTS); lightIndex++)
    {
        FORWARD_PLUS_SUBTRACTIVE_LIGHT_CHECK
        Light light = GetAdditionalLight(lightIndex, WorldPosition, Shadowmask);
    #ifdef _LIGHT_LAYERS
        if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
    #endif
        {
            // Precompute attenuated light color
            float3 attenuatedLightColor = light.color * (light.distanceAttenuation * light.shadowAttenuation);

            // Accumulate diffuse and specular lighting
            diffuseColor += LightingLambert(attenuatedLightColor, light.direction, WorldNormal);
            specularColor += LightingSpecular(attenuatedLightColor, light.direction, WorldNormal, WorldView, float4(SpecColor, 0), Smoothness);
        }
    }
    #endif

    // Setup input data for the light loop
    InputData inputData = (InputData)0;
    inputData.normalizedScreenSpaceUV = normalizedScreenSpaceUV;
    inputData.positionWS = WorldPosition;

    // Light loop for non-Forward+ rendering
    LIGHT_LOOP_BEGIN(pixelLightCount)
        Light light = GetAdditionalLight(lightIndex, WorldPosition, Shadowmask);
    #ifdef _LIGHT_LAYERS
        if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
    #endif
        {
            // Precompute attenuated light color
            float3 attenuatedLightColor = light.color * (light.distanceAttenuation * light.shadowAttenuation);

            // Accumulate diffuse and specular lighting
            diffuseColor += LightingLambert(attenuatedLightColor, light.direction, WorldNormal);
            specularColor += LightingSpecular(attenuatedLightColor, light.direction, WorldNormal, WorldView, float4(SpecColor, 0), Smoothness);
        }
    LIGHT_LOOP_END
#endif

    // Output results
    Diffuse = diffuseColor;
    Specular = specularColor;
}

//------------------------------------------------------------------------------------------------------
// Additional Lights Toon Example
//------------------------------------------------------------------------------------------------------

/*
- Calculates light attenuation values to produce multiple bands for a toon effect. See AdditionalLightsToon function below
*/
#ifndef SHADERGRAPH_PREVIEW
float ToonAttenuation(int lightIndex, float3 positionWS, float pointBands, float spotBands)
{
    // Fetch light data
    #if !USE_FORWARD_PLUS
        lightIndex = GetPerObjectLightIndex(lightIndex);
    #endif

    #if USE_STRUCTURED_BUFFER_FOR_LIGHT_DATA
        float4 lightPositionWS = _AdditionalLightsBuffer[lightIndex].position;
        half4 spotDirection = _AdditionalLightsBuffer[lightIndex].spotDirection;
        half4 distanceAndSpotAttenuation = _AdditionalLightsBuffer[lightIndex].attenuation;
    #else
        float4 lightPositionWS = _AdditionalLightsPosition[lightIndex];
        half4 spotDirection = _AdditionalLightsSpotDir[lightIndex];
        half4 distanceAndSpotAttenuation = _AdditionalLightsAttenuation[lightIndex];
    #endif

    // Calculate light vector and distance
    float3 lightVector = lightPositionWS.xyz - positionWS * lightPositionWS.w;
    float distanceSqr = max(dot(lightVector, lightVector), HALF_MIN);
    float range = rsqrt(distanceAndSpotAttenuation.x);
    float dist = sqrt(distanceSqr) * range; // Multiply instead of dividing by range

    // Spot light calculations
    half3 lightDirection = lightVector * rsqrt(distanceSqr);
    half SdotL = dot(spotDirection.xyz, lightDirection);
    half spotAtten = saturate(SdotL * distanceAndSpotAttenuation.z + distanceAndSpotAttenuation.w);
    spotAtten *= spotAtten; // Square for smoother falloff
    float maskSpotToRange = step(dist, 1); // Mask for spot light range

    // Determine if the light is a spot light
    bool isSpot = (distanceAndSpotAttenuation.z > 0);

    // Apply toon banding
    return isSpot ?
        (floor(spotAtten * spotBands) / spotBands) * maskSpotToRange : // Spot light banding
        saturate(1.0 - floor(dist * pointBands) / pointBands);         // Point light banding
}
#endif

void AdditionalLightsToon_float(
    float3 SpecColor, float Smoothness, float3 WorldPosition, float3 WorldNormal, float3 WorldView, half4 Shadowmask,
    float PointLightBands, float SpotLightBands, out float3 Diffuse, out float3 Specular) 
{
    float3 diffuseColor = 0;
    Specular = 0; // Specular is unused, so set it to 0 directly

#ifndef SHADERGRAPH_PREVIEW
    uint pixelLightCount = GetAdditionalLightsCount();
    uint meshRenderingLayers = GetMeshRenderingLayer();

    // Precompute the band condition to avoid branching inside the loop
    bool useBands = (PointLightBands > 1 || SpotLightBands > 1);

    #if USE_FORWARD_PLUS
    for (uint lightIndex = 0; lightIndex < min(URP_FP_DIRECTIONAL_LIGHTS_COUNT, MAX_VISIBLE_LIGHTS); lightIndex++) {
        FORWARD_PLUS_SUBTRACTIVE_LIGHT_CHECK
        Light light = GetAdditionalLight(lightIndex, WorldPosition, Shadowmask);
    #ifdef _LIGHT_LAYERS
        if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
    #endif
        {
            float attenuation = light.distanceAttenuation * light.shadowAttenuation;
            if (useBands) {
                diffuseColor += light.color * light.shadowAttenuation * 
                    ToonAttenuation(lightIndex, WorldPosition, PointLightBands, SpotLightBands);
            } else {
                diffuseColor += light.color * (attenuation > 0.0001 ? 1.0 : 0.0);
            }
        }
    }
    #endif

    // Only compute inputData if needed for the light loop
    #if !USE_FORWARD_PLUS
    InputData inputData = (InputData)0;
    float4 screenPos = ComputeScreenPos(TransformWorldToHClip(WorldPosition));
    inputData.normalizedScreenSpaceUV = screenPos.xy / screenPos.w;
    inputData.positionWS = WorldPosition;

    LIGHT_LOOP_BEGIN(pixelLightCount)
        Light light = GetAdditionalLight(lightIndex, WorldPosition, Shadowmask);
    #ifdef _LIGHT_LAYERS
        if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
    #endif
        {
            float attenuation = light.distanceAttenuation * light.shadowAttenuation;
            if (useBands) {
                diffuseColor += light.color * light.shadowAttenuation * 
                    ToonAttenuation(lightIndex, WorldPosition, PointLightBands, SpotLightBands);
            } else {
                diffuseColor += light.color * (attenuation > 0.0001 ? 1.0 : 0.0);
            }
        }
    LIGHT_LOOP_END
    #endif
#endif

    Diffuse = diffuseColor;
}
#endif // CUSTOM_LIGHTING_INCLUDED
