using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Runtime.Systems.Persistence.Editor
{
    [CustomEditor(typeof(SaveLoadService))]
    public class SaveManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var saveLoadSystem = (SaveLoadService)target;
            var gameName = saveLoadSystem.GameData.Name;

            DrawDefaultInspector();

            if (GUILayout.Button("New Game")) saveLoadSystem.NewGame();
            if (GUILayout.Button("Save Game")) saveLoadSystem.SaveGame();
            if (GUILayout.Button("Load Game")) saveLoadSystem.LoadGame(gameName);
            if (GUILayout.Button("Delete Game")) saveLoadSystem.DeleteGame(gameName);
        }
    }
}