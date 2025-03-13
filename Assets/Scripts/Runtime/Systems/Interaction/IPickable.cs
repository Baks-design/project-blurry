using UnityEngine;

namespace Assets.Scripts.Runtime.Systems.Interaction
{
    public interface IPickable
    {
        bool IsPicked { get; set; }

        void OnPickUp(Vector3 position, Quaternion rotation);
        void OnManipulate(Vector2 input);
        void OnAddInventory(Vector3 playePos);
    }
}