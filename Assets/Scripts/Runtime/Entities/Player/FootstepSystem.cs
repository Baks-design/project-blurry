using AudioSystem;
using KBCore.Refs;
using UnityEngine;

namespace Assets.Scripts.Runtime.Entities.Player
{
    public class FootstepSystem : MonoBehaviour
    {
        [SerializeField, Parent] AdvancedGridMovement advancedGridMovement;
        [SerializeField, Anywhere] Transform rightFoot, leftFoot;
        [SerializeField] SoundData stepSoundData, blockSoundData;
        bool _nextStepLeft;
        SoundBuilder soundBuilder;

        void Start() => soundBuilder = SoundManager.Instance.CreateSoundBuilder();

        void OnEnable()
        {
            advancedGridMovement.BlockedEvent += Block;
            advancedGridMovement.StepEvent += Step;
            advancedGridMovement.TurnEvent += Turn;
        }

        void OnDisable()
        {
            advancedGridMovement.BlockedEvent -= Block;
            advancedGridMovement.StepEvent -= Step;
            advancedGridMovement.TurnEvent -= Turn;
        }

        void Step()
        {
            var foot = _nextStepLeft ? leftFoot : rightFoot;
            soundBuilder.WithRandomPitch().WithPosition(foot.position).Play(stepSoundData);
            _nextStepLeft = !_nextStepLeft;
        }

        void Turn()
        {
            soundBuilder.WithRandomPitch().WithPosition(leftFoot.position).Play(stepSoundData);
            soundBuilder.WithRandomPitch().WithPosition(rightFoot.position).Play(stepSoundData);
        }

        void Block() => soundBuilder.Play(blockSoundData);
    }
}