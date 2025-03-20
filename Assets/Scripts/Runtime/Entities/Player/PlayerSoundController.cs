using System;
using Assets.Scripts.Runtime.Systems.Audio;
using Assets.Scripts.Runtime.Utilities.Patterns.ServicesLocator;
using KBCore.Refs;
using UnityEngine;

namespace Assets.Scripts.Runtime.Entities.Player
{
    public class PlayerSoundController : MonoBehaviour
    {
        [SerializeField, Parent] PlayerMovementController movementController;
        [SerializeField, Anywhere] Transform rightFoot, leftFoot;
        [SerializeField] GroundTypeSound[] groundTypeSounds;
        [Serializable]
        public struct GroundTypeSound
        {
            public GroundType groundType;
            public SoundData soundData;
        }
        bool _nextStepLeft;
        SoundBuilder _soundBuilder;
        ISoundService soundService;

        void Start()
        {
            ServiceLocator.Global.Get(out soundService);
            _soundBuilder = soundService.CreateSoundBuilder();
        }

        void OnEnable()
        {
            movementController.OnStep += PlayStepSound;
            movementController.OnRun += PlayStepSound;
        }

        void OnDisable()
        {
            movementController.OnStep -= PlayStepSound;
            movementController.OnRun -= PlayStepSound;
        }

        void PlayStepSound()
        {
            if (!IsGrounded(out Transform foot, out GroundType groundType)) return;

            var soundToPlay = GetSoundForGroundType(groundType);
            _soundBuilder.WithRandomPitch().WithPosition(foot.position).Play(soundToPlay);

            _nextStepLeft = !_nextStepLeft;
        }

        bool IsGrounded(out Transform foot, out GroundType groundType)
        {
            foot = _nextStepLeft ? leftFoot : rightFoot;

            if (Physics.Raycast(foot.position, Vector3.down, out var hit))
            {
                if (hit.collider.TryGetComponent(out AudioTag audioTag))
                {
                    groundType = audioTag.GroundTypeTag;
                    return true;
                }
            }

            groundType = GroundType.None;
            return false;
        }

        SoundData GetSoundForGroundType(GroundType groundType)
        {
            foreach (var groundTypeSound in groundTypeSounds)
                if (groundTypeSound.groundType == groundType)
                    return groundTypeSound.soundData;
            return null;
        }
    }
}