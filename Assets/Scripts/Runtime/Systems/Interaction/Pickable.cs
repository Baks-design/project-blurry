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

        public bool IsPicked { get; set; }

        public void OnPickUp(Vector3 position, Quaternion rotation, Vector2 input)
        {
            Debug.Log($"PickUp: {this}");

            IsPicked = true;

            var finalPos = position + Camera.main.transform.forward * distanceFromCamera;
            tr.SetPositionAndRotation(Vector3.Lerp(tr.position, finalPos, positionSpeed * Time.deltaTime), rotation);

            var targetEulerAngles = new Vector3(-input.y, input.x, 0f);
            var targetRotation = Quaternion.Euler(targetEulerAngles);
            tr.localRotation = Quaternion.Slerp(tr.localRotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        public async void OnSave(Vector3 playerPos)
        {
            Debug.Log($"Dropped: {this}");

            IsPicked = false;

            coll.enabled = false;

            while (Vector3.Distance(tr.position, playerPos) > 0.1f)
            {
                tr.position = Vector3.Lerp(tr.position, playerPos, positionSpeed * Time.deltaTime);
                await Awaitable.NextFrameAsync();
            }

            Destroy(gameObject);
        }

        public void OnDrop(Vector3 position)
        {
            //
        }
    }
}