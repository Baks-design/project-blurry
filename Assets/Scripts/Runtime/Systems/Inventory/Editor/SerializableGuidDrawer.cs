using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using Assets.Scripts.Runtime.Systems.Inventory.Helpers;

namespace Assets.Scripts.Runtime.Systems.Inventory.Editor
{
    [CustomPropertyDrawer(typeof(SerializableGuid))]
    public class SerializableGuidDrawer : PropertyDrawer
    {
        static readonly string[] GuidParts = { "Part1", "Part2", "Part3", "Part4" };

        static SerializedProperty[] GetGuidParts(SerializedProperty property)
        {
            var values = new SerializedProperty[GuidParts.Length];

            for (var i = 0; i < GuidParts.Length; i++)
                values[i] = property.FindPropertyRelative(GuidParts[i]);

            return values;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var guidParts = GetGuidParts(property);
            var areAllPartsValid = true;

            // Check if all GUID parts are valid (non-null)
            for (var i = 0; i < guidParts.Length; i++)
            {
                if (guidParts[i] == null)
                {
                    areAllPartsValid = false;
                    break;
                }
            }

            if (areAllPartsValid)
                EditorGUI.LabelField(position, BuildGuidString(guidParts));
            else
                EditorGUI.SelectableLabel(position, "GUID Not Initialized");

            var hasClicked = Event.current.type == EventType.MouseUp && Event.current.button == 1;
            if (hasClicked && position.Contains(Event.current.mousePosition))
            {
                ShowContextMenu(property);
                Event.current.Use();
            }

            EditorGUI.EndProperty();
        }

        void ShowContextMenu(SerializedProperty property)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Copy GUID"), false, () => CopyGuid(property));
            menu.AddItem(new GUIContent("Reset GUID"), false, () => ResetGuid(property));
            menu.AddItem(new GUIContent("Regenerate GUID"), false, () => RegenerateGuid(property));
            menu.ShowAsContext();
        }

        void CopyGuid(SerializedProperty property)
        {
            var guidParts = GetGuidParts(property);
            var areAllPartsValid = true;

            // Check if all GUID parts are valid (non-null)
            for (var i = 0; i < guidParts.Length; i++)
            {
                if (guidParts[i] == null)
                {
                    areAllPartsValid = false;
                    break;
                }
            }

            if (!areAllPartsValid) return;

            var guid = BuildGuidString(guidParts);
            EditorGUIUtility.systemCopyBuffer = guid;
            Debug.Log($"GUID copied to clipboard: {guid}");
        }

        void ResetGuid(SerializedProperty property)
        {
            const string warning = "Are you sure you want to reset the GUID?";
            if (!EditorUtility.DisplayDialog("Reset GUID", warning, "Yes", "No")) return;

            var guidParts = GetGuidParts(property);
            for (var i = 0; i < guidParts.Length; i++)
                guidParts[i].uintValue = 0;

            property.serializedObject.ApplyModifiedProperties();
            Debug.Log("GUID has been reset.");
        }

        void RegenerateGuid(SerializedProperty property)
        {
            const string warning = "Are you sure you want to regenerate the GUID?";
            if (!EditorUtility.DisplayDialog("Reset GUID", warning, "Yes", "No")) return;

            var bytes = Guid.NewGuid().ToByteArray();
            var guidParts = GetGuidParts(property);

            for (var i = 0; i < GuidParts.Length; i++)
                guidParts[i].uintValue = BitConverter.ToUInt32(bytes, i * 4);

            property.serializedObject.ApplyModifiedProperties();
            Debug.Log("GUID has been regenerated.");
        }

        static string BuildGuidString(SerializedProperty[] guidParts)
        {
            var sb = new StringBuilder();
            
            for (var i = 0; i < guidParts.Length; i++)
                sb.AppendFormat("{0:X8}", guidParts[i].uintValue);

            return sb.ToString();
        }
    }
}