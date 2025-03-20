using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Assets.Scripts.Runtime.Systems.SceneManagement
{
    public readonly struct AsyncOperationHandleGroup
    {
        public readonly List<AsyncOperationHandle<SceneInstance>> Handles;

        public float Progress
        {
            get
            {
                if (Handles.Count == 0) return 0;

                var totalProgress = 0f;
                foreach (var handle in Handles)
                    totalProgress += handle.PercentComplete;

                return totalProgress / Handles.Count;
            }
        }
        public bool IsDone
        {
            get
            {
                if (Handles.Count == 0) return true;

                foreach (var handle in Handles)
                    if (!handle.IsDone)
                        return false;

                return true;
            }
        }

        public AsyncOperationHandleGroup(int initialCapacity)
        => Handles = new List<AsyncOperationHandle<SceneInstance>>(initialCapacity);
    }
}