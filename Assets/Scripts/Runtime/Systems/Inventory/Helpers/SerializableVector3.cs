using UnityEngine;

namespace Assets.Scripts.Runtime.Systems.Inventory.Helpers
{
    /// <summary>
    /// Represents a serializable version of the Unity Vector3 struct.
    /// </summary>
    public struct SerializableVector3
    {
        public float x, y, z;

        public SerializableVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        
        public override readonly string ToString() => $"[{x}, {y}, {z}]";

        public static implicit operator Vector3(SerializableVector3 vector) => new(vector.x, vector.y, vector.z);

        public static implicit operator SerializableVector3(Vector3 vector) => new(vector.x, vector.y, vector.z);
    }
}