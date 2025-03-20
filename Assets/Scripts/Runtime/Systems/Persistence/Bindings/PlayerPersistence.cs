using Assets.Scripts.Runtime.Systems.Inventory.Helpers;
using KBCore.Refs;
using UnityEngine;

namespace Assets.Scripts.Runtime.Systems.Persistence.Bindings
{
    public class PlayerPersistence : MonoBehaviour, IBind<PlayerData>
    {
        [SerializeField, Self] Transform tr;
        [SerializeField] PlayerData data;

        [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();

        public void Bind(PlayerData data)
        {
            this.data = data;
            this.data.Id = Id;

            tr.SetPositionAndRotation(data.position, data.rotation);
        }

        void Update()
        {
            data.position = tr.position;
            data.rotation = tr.rotation;
        }
    }
}