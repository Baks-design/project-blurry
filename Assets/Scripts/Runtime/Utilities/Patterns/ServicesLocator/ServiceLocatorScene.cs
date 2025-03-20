using UnityEngine;

namespace Assets.Scripts.Runtime.Utilities.Patterns.ServicesLocator
{
    [AddComponentMenu("ServiceLocator/ServiceLocator Scene")]
    public class ServiceLocatorScene : Bootstrapper
    {
        protected override void Bootstrap() => Container.ConfigureForScene();
    }
}