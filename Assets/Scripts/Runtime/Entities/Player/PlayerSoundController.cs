using Assets.Scripts.Runtime.Systems.Audio;
using KBCore.Refs;
using UnityEngine;

namespace Assets.Scripts.Runtime.Entities.Player
{
    public class PlayerSoundController : MonoBehaviour
    {
        [SerializeField, Parent] PlayerMovementController movementController;
        [SerializeField, Anywhere] Transform rightFoot, leftFoot;
        [SerializeField, Anywhere] SoundData stepSoundData, blockSoundData;
        bool _nextStepLeft;
        SoundBuilder soundBuilder;

        void Start() => soundBuilder = SoundManager.Instance.CreateSoundBuilder();

        void OnEnable()
        {
            movementController.BlockedEvent += Block;
            movementController.StepEvent += Step;
            movementController.TurnEvent += Turn;
        }

        void OnDisable()
        {
            movementController.BlockedEvent -= Block;
            movementController.StepEvent -= Step;
            movementController.TurnEvent -= Turn;
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