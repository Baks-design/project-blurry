using UnityEngine;

namespace Assets.Scripts.Runtime.Systems.Interaction
{
    public interface IPickable
    {
        bool IsPicked { get; set; }

        void OnPickUp(Vector3 position, Quaternion rotation, Vector2 input);
        void OnSave(Vector3 playePos);
        void OnDrop(Vector3 position);
    }
}