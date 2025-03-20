using System.Collections.Generic;
using Assets.Scripts.Runtime.Utilities.Patterns.ServicesLocator;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.Pool;

namespace Assets.Scripts.Runtime.Systems.Audio
{
    public class SoundService : MonoBehaviour, ISoundService
    {
        public readonly LinkedList<SoundEmitter> FrequentSoundEmitters = new();
        
        [SerializeField, Anywhere] SoundEmitter soundEmitterPrefab;
        [SerializeField] bool collectionCheck = true;
        [SerializeField] int defaultCapacity = 10;
        [SerializeField] int maxPoolSize = 100;
        [SerializeField] int maxSoundInstances = 30;
        IObjectPool<SoundEmitter> soundEmitterPool;
        readonly List<SoundEmitter> activeSoundEmitters = new();

        void Awake()
        {
            DontDestroyOnLoad(this);
            ServiceLocator.Global.Register<ISoundService>(this);
        }

        void Start() => InitializePool();

        public SoundBuilder CreateSoundBuilder() => new(this);

        public bool CanPlaySound(SoundData data)
        {
            if (!data.frequentSound) return true;

            if (FrequentSoundEmitters.Count >= maxSoundInstances)
            {
                try
                {
                    FrequentSoundEmitters.First.Value.Stop();
                    return true;
                }
                catch
                {
                    Debug.Log("SoundEmitter is already released");
                }
                return false;
            }
            return true;
        }

        public SoundEmitter Get() => soundEmitterPool.Get();

        public void ReturnToPool(SoundEmitter soundEmitter) => soundEmitterPool.Release(soundEmitter);

        public void StopAll()
        {
            foreach (var soundEmitter in activeSoundEmitters)
                soundEmitter.Stop();

            FrequentSoundEmitters.Clear();
        }

        void InitializePool()
        => soundEmitterPool = new ObjectPool<SoundEmitter>(
            CreateSoundEmitter, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject,
            collectionCheck, defaultCapacity, maxPoolSize);

        SoundEmitter CreateSoundEmitter()
        {
            var soundEmitter = Instantiate(soundEmitterPrefab);
            soundEmitter.gameObject.SetActive(false);
            return soundEmitter;
        }

        void OnTakeFromPool(SoundEmitter soundEmitter)
        {
            soundEmitter.gameObject.SetActive(true);
            activeSoundEmitters.Add(soundEmitter);
        }

        void OnReturnedToPool(SoundEmitter soundEmitter)
        {
            if (soundEmitter.Node != null)
            {
                FrequentSoundEmitters.Remove(soundEmitter.Node);
                soundEmitter.Node = null;
            }
            soundEmitter.gameObject.SetActive(false);
            activeSoundEmitters.Remove(soundEmitter);
        }

        void OnDestroyPoolObject(SoundEmitter soundEmitter)
        {
            if (soundEmitter != null)
                Destroy(soundEmitter.gameObject);
        }
    }
}