using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Runtime.Systems.EventBus
{
    /// <summary>
    /// Contains methods and properties related to event buses and event types in the Unity application.
    /// </summary>
    /// <summary>
    /// Contains methods and properties related to event buses and event types in the Unity application.
    /// </summary>
    public static class EventBusUtil
    {
        public static IReadOnlyList<Type> EventTypes { get; set; }
        public static IReadOnlyList<Type> EventBusTypes { get; set; }

#if UNITY_EDITOR
        public static PlayModeStateChange PlayModeState { get; set; }

        /// <summary>
        /// Initializes the Unity Editor related components of the EventBusUtil.
        /// </summary>    
        [InitializeOnLoadMethod]
        public static void RegisterEditor()
        {
            // Ensure the event handler is only registered once
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged; // Deregister first to avoid duplicates
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        [RuntimeInitializeOnLoadMethod]
        public static void DeregisterEditor() => EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;

        static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            PlayModeState = state;
            if (state == PlayModeStateChange.ExitingPlayMode)
                ClearAllBuses();
        }
#endif

        /// <summary>
        /// Initializes the EventBusUtil class at runtime before the loading of any scene.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            EventTypes = PredefinedAssemblyUtil.GetTypes(typeof(IEvent));
            EventBusTypes = InitializeAllBuses();
        }

        static List<Type> InitializeAllBuses()
        {
            var eventBusTypes = new List<Type>();

            foreach (var eventType in EventTypes)
            {
                var busType = typeof(EventBus<>).MakeGenericType(eventType);
                eventBusTypes.Add(busType);
                Debug.Log($"Initialized EventBus<{eventType.Name}>");
            }

            return eventBusTypes;
        }

        /// <summary>
        /// Clears (removes all listeners from) all event buses in the application.
        /// </summary>
        public static void ClearAllBuses()
        {
            Debug.Log("Clearing all buses...");
            foreach (var busType in EventBusTypes)
            {
                var clearMethod = typeof(EventBusUtil).GetMethod(nameof(ClearBus), BindingFlags.Static | BindingFlags.NonPublic)
                    ?.MakeGenericMethod(busType.GenericTypeArguments[0]);
                clearMethod?.Invoke(null, null);
            }
        }

        static void ClearBus<T>() where T : IEvent => EventBus<T>.Clear();
    }
}