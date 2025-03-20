using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System;

namespace Assets.Scripts.Runtime.Systems.Inventory
{
    public abstract class StorageView : MonoBehaviour
    {
        [SerializeField] protected UIDocument document;
        [SerializeField] protected StyleSheet styleSheet;
        public Slot[] Slots;
        static bool isDragging;
        static Slot originalSlot;
        protected static VisualElement ghostIcon;
        protected VisualElement root;
        protected VisualElement container;

        public event Action<Slot, Slot> OnDrop = delegate { };

        [RuntimeInitializeOnLoadMethod]
        static void OnRuntimeMethodLoad()
        {
            ghostIcon = new VisualElement();
            isDragging = false;
            originalSlot = null;
        }

        void Start()
        {
            ghostIcon.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            ghostIcon.RegisterCallback<PointerUpEvent>(OnPointerUp);

            foreach (var slot in Slots)
                slot.OnStartDrag += OnPointerDown;
        }

        public abstract IEnumerator InitializeView(ViewModel viewModel);

        static void OnPointerDown(Vector2 position, Slot slot)
        {
            isDragging = true;
            originalSlot = slot;

            SetGhostIconPosition(position);

            ghostIcon.style.backgroundImage = originalSlot.BaseSprite.texture;
            originalSlot.Icon.image = null;
            originalSlot.StackLabel.visible = false;

            ghostIcon.style.visibility = Visibility.Visible;
            // TODO show stack size on ghost icon
        }

        void OnPointerMove(PointerMoveEvent evt)
        {
            if (!isDragging) return;

            SetGhostIconPosition(evt.position);
        }

        void OnPointerUp(PointerUpEvent evt)
        {
            if (!isDragging) return;

            Slot closestSlot = null;
            var closestDistance = float.MaxValue;

            // Find the closest slot that overlaps with the ghost icon
            foreach (var slot in Slots)
            {
                if (slot.worldBound.Overlaps(ghostIcon.worldBound))
                {
                    var distance = Vector2.Distance(slot.worldBound.position, ghostIcon.worldBound.position);
                    if (distance < closestDistance)
                    {
                        closestSlot = slot;
                        closestDistance = distance;
                    }
                }
            }

            if (closestSlot != null)
                OnDrop?.Invoke(originalSlot, closestSlot);
            else
                originalSlot.Icon.image = originalSlot.BaseSprite.texture;

            isDragging = false;
            originalSlot = null;
            ghostIcon.style.visibility = Visibility.Hidden;
        }

        static void SetGhostIconPosition(Vector2 position)
        {
            ghostIcon.style.top = position.y - ghostIcon.layout.height / 2f;
            ghostIcon.style.left = position.x - ghostIcon.layout.width / 2f;
        }

        void OnDestroy()
        {
            foreach (var slot in Slots)
                slot.OnStartDrag -= OnPointerDown;
        }
    }
}