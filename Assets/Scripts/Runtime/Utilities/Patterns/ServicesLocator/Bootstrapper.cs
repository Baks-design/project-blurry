using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts.Runtime.Utilities.Patterns.ServicesLocator
{
    [DisallowMultipleComponent, RequireComponent(typeof(ServiceLocator))]
    public abstract class Bootstrapper : MonoBehaviour
    {
        bool hasBeenBootstrapped;
        ServiceLocator container;

        internal ServiceLocator Container
        {
            get
            {
                if (container == null)
                    if (!TryGetComponent(out container))
                        Assert.IsNull(container, "ServiceLocator component not found!");
                return container;
            }
        }

        void Awake() => BootstrapOnDemand();

        public void BootstrapOnDemand()
        {
            if (hasBeenBootstrapped) return;
            hasBeenBootstrapped = true;
            Bootstrap();
        }

        protected abstract void Bootstrap();
    }
}
