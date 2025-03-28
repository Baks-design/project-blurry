using System;
//using Assets.Scripts.Runtime.Systems.Inventory;

namespace Assets.Scripts.Runtime.Systems.Persistence
{
    [Serializable]
    public class GameData
    {
        public string Name;
        public string CurrentLevelName;
        public PlayerData playerData;
        //public InventoryData inventoryData;
    }
}