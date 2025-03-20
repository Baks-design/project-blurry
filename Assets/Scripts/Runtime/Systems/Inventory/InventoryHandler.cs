using System.Collections.Generic;
using Assets.Scripts.Runtime.Systems.Inventory.Helpers;
using Assets.Scripts.Runtime.Systems.Persistence;
using UnityEngine;

namespace Assets.Scripts.Runtime.Systems.Inventory
{
    public class InventoryHandler : MonoBehaviour, IBind<InventoryData>
    {
        [SerializeField] InventoryView view;
        [SerializeField] int capacity = 20;
        [SerializeField] List<ItemDetails> startingItems = new();
        InventoryController controller;
        
        [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();

        void Awake() => controller = new Builder(view)
            .WithStartingItems(startingItems)
            .WithCapacity(capacity)
            .Build();

        public void Bind(InventoryData data)
        {
            controller.Bind(data);
            data.Id = Id;
        }
    }
}