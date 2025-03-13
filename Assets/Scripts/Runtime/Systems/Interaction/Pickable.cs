using KBCore.Refs;
using UnityEngine;

namespace Assets.Scripts.Runtime.Systems.Interaction
{
    public class Pickable : MonoBehaviour, IPickable
    {
        [SerializeField, Self] Transform tr;
        [SerializeField, Self] Collider coll;
        [SerializeField, Range(1f, 10f)] float distanceFromCamera = 3f;
        [SerializeField, Range(1f, 10f)] float positionSpeed = 2f;
        [SerializeField, Range(1f, 10f)] float rotationSpeed = 2f;
        private Vector3 initPos;


        public bool IsPicked { get; set; }

        public void Start()
        {
            initPos = tr.localPosition;
        }

        public void OnPickUp(Vector3 position, Quaternion rotation)
        {
            Debug.Log($"PickUp: {this}");

            IsPicked = true;

            var finalPos = new Vector3(position.x, position.y, position.z + distanceFromCamera);
            tr.SetLocalPositionAndRotation(Vector3.Lerp(tr.localPosition, finalPos, positionSpeed * Time.deltaTime), rotation);
        }

        public void OnManipulate(Vector2 input)
        {
            Debug.Log($"Manipulating: {this}");

            var targetEulerAngles = new Vector2(input.x, input.y);
            var targetRotation = Quaternion.Euler(targetEulerAngles);
            tr.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        public async void OnAddInventory(Vector3 playePos)
        {
            Debug.Log($"Dropped: {this}");

            IsPicked = false;

            coll.enabled = false;
            tr.localPosition = Vector3.Lerp(tr.localPosition, playePos, positionSpeed * Time.deltaTime);
            await WaitForDestroy();
        }

        async Awaitable WaitForDestroy()
        {
            while (initPos == tr.localPosition)
            {
                await Awaitable.NextFrameAsync();
            }

            Destroy(this);
        }
    }
}