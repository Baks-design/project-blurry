using UnityEngine;

namespace Assets.Scripts.Runtime.Systems.Interaction
{
    public interface IPickable
    {
        bool IsPicked { get; set; }
        bool IsHolded { get; set; }
        bool IsReleased { get; set; }
        Rigidbody Rigid { get; set; }

        void OnPickUp();
        void OnManipulate();
        void OnRelease();
    }
}