using System.IO;
using Assets.Scripts.Runtime.Systems.Inventory.Helpers;

namespace Assets.Scripts.Runtime.Systems.Inventory.Extensions
{
    public static class BinaryReaderExtensions
    {
        public static SerializableGuid Read(this BinaryReader reader)
        => new(reader.ReadUInt32(), reader.ReadUInt32(), reader.ReadUInt32(), reader.ReadUInt32());
    }
}