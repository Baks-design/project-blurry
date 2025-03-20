using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Runtime.Systems.SceneManagement.Editor
{
    [CustomEditor(typeof(SceneLoaderService))]
    public class SceneLoaderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var sceneLoader = (SceneLoaderService)target;

            if (EditorApplication.isPlaying && GUILayout.Button("Load First Scene Group"))
                LoadSceneGroup(sceneLoader, 0);
                
            if (EditorApplication.isPlaying && GUILayout.Button("Load Second Scene Group"))
                LoadSceneGroup(sceneLoader, 1);
        }

        static async void LoadSceneGroup(SceneLoaderService sceneLoader, int index)
        => await sceneLoader.LoadSceneGroup(index);
    }
}