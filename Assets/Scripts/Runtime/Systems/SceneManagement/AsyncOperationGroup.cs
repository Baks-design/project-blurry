using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Runtime.Systems.SceneManagement
{
    public readonly struct AsyncOperationGroup
    {
        public readonly List<AsyncOperation> Operations;

        public float Progress
        {
            get
            {
                if (Operations.Count == 0) return 0;

                var totalProgress = 0f;
                foreach (var operation in Operations)
                    totalProgress += operation.progress;

                return totalProgress / Operations.Count;
            }
        }
        public bool IsDone
        {
            get
            {
                foreach (var operation in Operations)
                    if (!operation.isDone)
                        return false;

                return true;
            }
        }

        public AsyncOperationGroup(int initialCapacity) => Operations = new List<AsyncOperation>(initialCapacity);
    }
}