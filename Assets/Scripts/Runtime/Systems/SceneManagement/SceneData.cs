using System;
using Eflatun.SceneReference;

namespace Assets.Scripts.Runtime.Systems.SceneManagement
{
    [Serializable]
    public class SceneData
    {
        public SceneReference Reference;
        public SceneType SceneType;

        public string Name => Reference.Name;
    }
}