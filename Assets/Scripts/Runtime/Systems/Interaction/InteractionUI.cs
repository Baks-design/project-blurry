using Assets.Scripts.Runtime.Systems.EventBus;
using Assets.Scripts.Runtime.Systems.EventBus.Events;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Runtime.Systems.Interaction
{
    public class InteractionUI : MonoBehaviour //TODO: cONTINUAR DAQUI
    {
        [SerializeField] TMP_Text interactText;
        [SerializeField] GameObject interactContainer;
        [SerializeField] GameObject crosshairContainer;

        EventBinding<InteractObjectNameEvent> interactObjectNameEventBinding;

        void OnEnable()
        {
            interactObjectNameEventBinding = new EventBinding<InteractObjectNameEvent>(HandleInteractObjectNameEvent);
            EventBus<InteractObjectNameEvent>.Register(interactObjectNameEventBinding);
        }

        void OnDisable() => EventBus<InteractObjectNameEvent>.Deregister(interactObjectNameEventBinding);

        void HandleInteractObjectNameEvent(InteractObjectNameEvent interactObjectNameEvent)
            => interactText.text = interactObjectNameEvent.objectName;

        public void ToggleInteract(bool toggle) => interactContainer.SetActive(toggle);

        public void ToggleCrosshair(bool toggle) => crosshairContainer.SetActive(toggle);
    }
}