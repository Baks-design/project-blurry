using Assets.Scripts.Runtime.Systems.Persistence;
using Assets.Scripts.Runtime.Utilities.Helpers;
using UnityEngine;

namespace Assets.Scripts.Runtime.Components.Gameplay
{
    public class SaveTrigger : MonoBehaviour
    {
        IDataService dataService;
        TagHandle playerTagHandle;
        readonly GameData gameData;

        void Start()
        {
            dataService = new FileDataService(new JsonSerializer());
            playerTagHandle = TagHandle.GetExistingTag(GameTags.PlayerTag);
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(playerTagHandle)) return;
            dataService.Save(gameData);
        }
    }
}