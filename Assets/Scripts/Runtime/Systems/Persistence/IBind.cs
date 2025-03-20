using Assets.Scripts.Runtime.Systems.Inventory.Helpers;

namespace Assets.Scripts.Runtime.Systems.Persistence
{
    public interface IBind<TData> where TData : ISaveable
    {
        SerializableGuid Id { get; set; }

        void Bind(TData data);
    }
}