using System;
using Assets.Scripts.Runtime.Systems.Inventory.Helpers;
using UnityEngine;

namespace Assets.Scripts.Runtime.Systems.Persistence
{
    [Serializable]
    public class PlayerData : ISaveable
    {
        public Vector3 position;
        public Quaternion rotation;
        
        [field: SerializeField] public SerializableGuid Id { get; set; }
    }
}