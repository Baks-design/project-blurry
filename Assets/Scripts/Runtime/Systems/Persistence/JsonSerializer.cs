using UnityEngine;

namespace Assets.Scripts.Runtime.Systems.Persistence
{
    public class JsonSerializer : ISerializer
    {
        public string Serialize<T>(T obj) => JsonUtility.ToJson(obj, true);

        public T Deserialize<T>(string json) => JsonUtility.FromJson<T>(json);
    }
}