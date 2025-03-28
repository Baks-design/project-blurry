using Assets.Scripts.Runtime.Utilities.Helpers;

namespace Assets.Scripts.Runtime.Systems.Persistence
{
    public interface ISaveable
    {
        SerializableGuid Id { get; set; }
    }
}