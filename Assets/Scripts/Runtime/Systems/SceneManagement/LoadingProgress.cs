using System;

namespace Assets.Scripts.Runtime.Systems.SceneManagement
{
    public class LoadingProgress : IProgress<float>
    {
        const float ratio = 1f;

        public event Action<float> OnProgressed = delegate { };

        public void Report(float value) => OnProgressed.Invoke(value / ratio);
    }
}