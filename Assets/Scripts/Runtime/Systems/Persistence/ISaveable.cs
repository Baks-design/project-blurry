using Assets.Scripts.Runtime.Systems.Inventory.Helpers;

namespace Assets.Scripts.Runtime.Systems.Persistence
{
    public interface ISaveable
    {
        SerializableGuid Id { get; set; }
    }
}