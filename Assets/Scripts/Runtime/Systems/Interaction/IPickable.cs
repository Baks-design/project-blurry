using UnityEngine;

namespace Assets.Scripts.Runtime.Systems.Interaction
{
    public interface IPickable
    {
        void OnInteract(Transform holdPosition);
        void OnHoverStart();
    }
}