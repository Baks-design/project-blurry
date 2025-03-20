using System.Collections;
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

            // Disable the collider to prevent physics interference
            if (coll != null)
                coll.enabled = false;

            // Update the object's position and rotation every frame while picked up
            StartCoroutine(UpdatePickedObject(input));
        }

        IEnumerator UpdatePickedObject(Vector2 input)
        {
            while (IsPicked)
            {
                // Calculate the desired position in front of the camera
                Vector3 finalPos = Camera.main.transform.position + Camera.main.transform.forward * distanceFromCamera;

                // Smoothly move the object to the desired position
                tr.position = Vector3.Lerp(tr.position, finalPos, positionSpeed * Time.deltaTime);

                // Calculate the desired rotation based on input
                Vector3 targetEulerAngles = new Vector3(-input.y, input.x, 0f);
                Quaternion targetRotation = Quaternion.Euler(targetEulerAngles);

                // Smoothly rotate the object to the desired rotation
                tr.rotation = Quaternion.Slerp(tr.rotation, Camera.main.transform.rotation * targetRotation, rotationSpeed * Time.deltaTime);

                yield return null; // Wait for the next frame
            }

            // Re-enable the collider when the object is dropped
            if (coll != null)
            {
                coll.enabled = true;
            }
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

            //Destroy(gameObject);
        }

        public void OnDrop(Vector3 position)
        {
            //
        }
    }
}