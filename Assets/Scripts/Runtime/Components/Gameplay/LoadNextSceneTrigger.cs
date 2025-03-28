using Assets.Scripts.Runtime.Systems.SceneManagement;
using Assets.Scripts.Runtime.Utilities.Helpers;
using Assets.Scripts.Runtime.Utilities.Patterns.ServicesLocator;
using UnityEngine;

namespace Assets.Scripts.Runtime.Components.Gameplay
{
    public class LoadNextSceneTrigger : MonoBehaviour
    {
        [SerializeField, TextArea] string whoNextSceneToLoad = "example";
        [SerializeField] int loadSceneGroup = 0;
        TagHandle playerTagHandle;
        ISceneLoaderService sceneLoaderService;

        void Start()
        {
            playerTagHandle = TagHandle.GetExistingTag(GameTags.PlayerTag);
            ServiceLocator.Global.Get(out sceneLoaderService);
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(playerTagHandle)) return;

            if (!string.IsNullOrEmpty(whoNextSceneToLoad))
                sceneLoaderService.LoadSceneGroup(loadSceneGroup);
        }
    }
}