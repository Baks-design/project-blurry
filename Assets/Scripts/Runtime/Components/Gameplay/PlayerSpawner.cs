using KBCore.Refs;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts.Runtime.Components.Gameplay
{
    public class PlayerSpawner : MonoBehaviour
    {
        [SerializeField, Self] Transform spawnPoint;
        [SerializeField, Anywhere] GameObject playerPrefab;

        void Awake() => SpawnPlayer();

        void SpawnPlayer()
        {
            if (playerPrefab != null && spawnPoint != null)
            {
                var player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
                DontDestroyOnLoad(player);
            }
            else
            {
                Assert.IsNull(playerPrefab, "Player prefab is not assigned!");
                Assert.IsNull(spawnPoint, "Player spawn point is not assigned!");
            }
        }
    }
}