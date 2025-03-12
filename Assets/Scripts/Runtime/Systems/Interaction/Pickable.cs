using KBCore.Refs;
using UnityEngine;

namespace Assets.Scripts.Runtime.Systems.Interaction
{
    [RequireComponent(typeof(Rigidbody))]
    public class Pickable : MonoBehaviour, IPickable
    {
        [SerializeField, Self] Rigidbody rigid;

        public Rigidbody Rigid { get; set; }
        public bool IsPicked { get; set; }
        public bool IsHolded { get; set; }
        public bool IsReleased { get; set; }

        public void OnPickUp()
        {
            IsPicked = true;
            Debug.Log($"PickUp: {this}");
        }

        public void OnManipulate()
        {
            Debug.Log($"Manipulating: {this}");
            //rotate
        }

        public void OnRelease()
        {
            IsPicked = false;
            Debug.Log($"Dropped: {this}");
            //drop
        }
    }
}