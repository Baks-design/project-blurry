using UnityEngine;

namespace Assets.Scripts.Runtime.Utilities.Extensions
{
    public static class GameObjectExtensions
    {
        public static T GetOrAdd<T>(this GameObject gameObject) where T : Component
        {
            var component = gameObject.GetComponent<T>();
            if (!component)
                component = gameObject.AddComponent<T>();
            return component;
        }
    }
}