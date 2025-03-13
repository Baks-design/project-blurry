using System.Collections.Generic;
using Assets.Scripts.Runtime.Utilities.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Runtime.Systems.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundEmitter : MonoBehaviour
    {
        AudioSource audioSource;

        public SoundData Data { get; private set; }
        public LinkedListNode<SoundEmitter> Node { get; set; }

        void Awake() => audioSource = gameObject.GetOrAdd<AudioSource>();

        public void Initialize(SoundData data)
        {
            Data = data;
            audioSource.clip = data.clip;
            audioSource.outputAudioMixerGroup = data.mixerGroup;
            audioSource.loop = data.loop;
            audioSource.playOnAwake = data.playOnAwake;
            audioSource.mute = data.mute;
            audioSource.bypassEffects = data.bypassEffects;
            audioSource.bypassListenerEffects = data.bypassListenerEffects;
            audioSource.bypassReverbZones = data.bypassReverbZones;
            audioSource.priority = data.priority;
            audioSource.volume = data.volume;
            audioSource.pitch = data.pitch;
            audioSource.panStereo = data.panStereo;
            audioSource.spatialBlend = data.spatialBlend;
            audioSource.reverbZoneMix = data.reverbZoneMix;
            audioSource.dopplerLevel = data.dopplerLevel;
            audioSource.spread = data.spread;
            audioSource.minDistance = data.minDistance;
            audioSource.maxDistance = data.maxDistance;
            audioSource.ignoreListenerVolume = data.ignoreListenerVolume;
            audioSource.ignoreListenerPause = data.ignoreListenerPause;
            audioSource.rolloffMode = data.rolloffMode;
        }

        public async void Play()
        {
            audioSource.Play();
            await WaitForSoundToEndAsync();
        }

        async Awaitable WaitForSoundToEndAsync()
        {
            while (audioSource.isPlaying)
                await Awaitable.NextFrameAsync();

            Stop();
        }

        public void Stop()
        {
            audioSource.Stop();
            SoundManager.Instance.ReturnToPool(this);
        }

        public void WithRandomPitch(float min = -0.05f, float max = 0.05f)
        => audioSource.pitch += Random.Range(min, max);
    }
}