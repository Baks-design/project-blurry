using System;
using UnityEngine;

namespace Systems.Inventory
{
    [Serializable]
    [CreateAssetMenu(menuName = "Inventory/Item")]
    public class ItemDetails : ScriptableObject
    {
        [BeginGroup("ItemSplit", Style = GroupStyle.Round)]
        public string Name;
        public int maxStack = 1;

        [EditorButton(nameof(AssignNewGuid), "<b>My</b> AssignNewGuid", activityType: ButtonActivityType.OnPlayMode,
        ValidateMethodName = nameof(ValidationMethod))]
        public SerializableGuid Id = SerializableGuid.NewGuid();

        [AssetPreview]
        public Sprite Icon;

        [EndGroup]
        [Label("My Custom Header", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
        public string Description;

        void AssignNewGuid() => Id = SerializableGuid.NewGuid();

        bool ValidationMethod() => Id != SerializableGuid.NewGuid();

        public Item Create(int quantity) => new(this, quantity);
    }
}