using System;
using System.Collections.Generic;
using Assets.Scripts.Runtime.Utilities.Extensions;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Runtime.Utilities.Patterns.ServicesLocator
{
    public class ServiceLocator : MonoBehaviour
    {
        static ServiceLocator global;
        static Dictionary<Scene, ServiceLocator> sceneContainers;
        static List<GameObject> tmpSceneGameObjects;
        readonly ServiceManager services = new();
        const string k_globalServiceLocatorName = "ServiceLocator [Global]";
        const string k_sceneServiceLocatorName = "ServiceLocator [Scene]";

        internal void ConfigureAsGlobal(bool dontDestroyOnLoad)
        {
            if (global == this)
                Debug.LogWarning("ServiceLocator.ConfigureAsGlobal: Already configured as global", this);
            else if (global != null)
                Debug.LogError("ServiceLocator.ConfigureAsGlobal: Another ServiceLocator is already configured as global", this);
            else
            {
                global = this;
                if (dontDestroyOnLoad)
                    DontDestroyOnLoad(gameObject);
            }
        }
        internal void ConfigureForScene()
        {
            var scene = gameObject.scene;

            if (sceneContainers.ContainsKey(scene))
            {
                Debug.LogError("ServiceLocator.ConfigureForScene: Another ServiceLocator is already configured for this scene", this);
                return;
            }

            sceneContainers.Add(scene, this);
        }

        public static ServiceLocator Global
        {
            get
            {
                if (global != null) return global;

                if (FindFirstObjectByType<ServiceLocatorGlobal>() is { } found)
                {
                    found.BootstrapOnDemand();
                    return global;
                }

                var container = new GameObject(k_globalServiceLocatorName, typeof(ServiceLocator));
                container.AddComponent<ServiceLocatorGlobal>().BootstrapOnDemand();

                return global;
            }
        }

        public static ServiceLocator ForSceneOf(MonoBehaviour mb)
        {
            var scene = mb.gameObject.scene;

            if (sceneContainers.TryGetValue(scene, out ServiceLocator container) && container != mb) return container;

            tmpSceneGameObjects.Clear();
            scene.GetRootGameObjects(tmpSceneGameObjects);

            foreach (var go in tmpSceneGameObjects)
            {
                if (go.TryGetComponent(out ServiceLocatorScene bootstrapper) && bootstrapper.Container != mb)
                {
                    bootstrapper.BootstrapOnDemand();
                    return bootstrapper.Container;
                }
            }

            return Global;
        }

        public static ServiceLocator For(MonoBehaviour mb)
        {
            var container = mb.GetComponentInParent<ServiceLocator>().OrNull();
            if (container != null) return container;

            container = ForSceneOf(mb);
            if (container != null)  return container;

            return Global;
        }

        public ServiceLocator Register<T>(T service)
        {
            services.Register(service);
            return this;
        }

        public ServiceLocator Register(Type type, object service)
        {
            services.Register(type, service);
            return this;
        }

        public ServiceLocator Get<T>(out T service) where T : class
        {
            if (TryGetService(out service)) return this;

            if (TryGetNextInHierarchy(out ServiceLocator container))
            {
                container.Get(out service);
                return this;
            }

            throw new ArgumentException($"ServiceLocator.Get: Service of type {typeof(T).FullName} not registered");
        }

        public T Get<T>() where T : class
        {
            var type = typeof(T);

            if (TryGetService(type, out T service)) return service;

            if (TryGetNextInHierarchy(out ServiceLocator container)) return container.Get<T>();

            throw new ArgumentException($"Could not resolve type '{typeof(T).FullName}'.");
        }

        public bool TryGet<T>(out T service) where T : class
        {
            var type = typeof(T);

            if (TryGetService(type, out service)) return true;

            return TryGetNextInHierarchy(out ServiceLocator container) && container.TryGet(out service);
        }

        bool TryGetService<T>(out T service) where T : class => services.TryGet(out service);

        bool TryGetService<T>(Type type, out T service) where T : class => services.TryGet(out service);

        bool TryGetNextInHierarchy(out ServiceLocator container)
        {
            if (this == global)
            {
                container = null;
                return false;
            }

            if (transform.parent != null)
            {
                container = transform.parent.GetComponentInParent<ServiceLocator>();
                if (container != null)  return true;
            }

            container = ForSceneOf(this);
            return container != null;
        }
        void OnDestroy()
        {
            if (this == global)
                global = null;
            else if (sceneContainers.ContainsValue(this))
                sceneContainers.Remove(gameObject.scene);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            global = null;
            sceneContainers = new Dictionary<Scene, ServiceLocator>();
            tmpSceneGameObjects = new List<GameObject>();
        }

#if UNITY_EDITOR
        [MenuItem("GameObject/ServiceLocator/Add Global")]
        static void AddGlobal()
        {
            var go = new GameObject(k_globalServiceLocatorName, typeof(ServiceLocatorGlobal));
        }

        [MenuItem("GameObject/ServiceLocator/Add Scene")]
        static void AddScene()
        {
            var go = new GameObject(k_sceneServiceLocatorName, typeof(ServiceLocatorScene));
        }
#endif
    }
}