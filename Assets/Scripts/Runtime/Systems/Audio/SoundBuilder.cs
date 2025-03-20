using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts.Runtime.Systems.Audio
{
    public class SoundBuilder
    {
        bool randomPitch;
        Vector3 position = Vector3.zero;
        readonly SoundService soundManager;

        public SoundBuilder(SoundService soundManager) => this.soundManager = soundManager;

        public SoundBuilder WithPosition(Vector3 position)
        {
            this.position = position;
            return this;
        }

        public SoundBuilder WithRandomPitch()
        {
            randomPitch = true;
            return this;
        }

        public void Play(SoundData soundData)
        {
            if (soundData == null)
            {
                Assert.IsNull(soundData, "SoundData is null");
                return;
            }

            if (!soundManager.CanPlaySound(soundData)) return;

            var soundEmitter = soundManager.Get();
            soundEmitter.Initialize(soundData);
            soundEmitter.transform.position = position;
            soundEmitter.transform.parent = soundManager.transform;

            if (randomPitch)
                soundEmitter.WithRandomPitch();

            if (soundData.frequentSound)
                soundEmitter.Node = soundManager.FrequentSoundEmitters.AddLast(soundEmitter);

            soundEmitter.Play();
        }
    }
}