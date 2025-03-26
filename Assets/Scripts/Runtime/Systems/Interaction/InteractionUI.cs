using Assets.Scripts.Runtime.Systems.EventBus;
using Assets.Scripts.Runtime.Systems.EventBus.Events;
using UnityEngine;

namespace Assets.Scripts.Runtime.Systems.Interaction
{
    public class InteractionUI : MonoBehaviour
    {
        string interactText;

        EventBinding<InteractObjectNameEvent> interactObjectNameEventBinding;

        void OnEnable()
        {
            interactObjectNameEventBinding = new EventBinding<InteractObjectNameEvent>(HandleInteractObjectNameEvent);
            EventBus<InteractObjectNameEvent>.Register(interactObjectNameEventBinding);
        }

        void OnDisable() => EventBus<InteractObjectNameEvent>.Deregister(interactObjectNameEventBinding);

        void HandleInteractObjectNameEvent(InteractObjectNameEvent interactObjectNameEvent)
        => interactText = interactObjectNameEvent.objectName;
    }
}