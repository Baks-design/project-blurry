using System;
using Assets.Scripts.Runtime.Systems.Inventory.Helpers;
using Assets.Scripts.Runtime.Systems.Persistence;
using UnityEngine;

namespace Assets.Scripts.Runtime.Systems.Inventory
{
    [Serializable]
    public class InventoryData : ISaveable
    {
        public int Capacity;
        public int Coins;
        public Item[] Items;

        [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();
    }
}