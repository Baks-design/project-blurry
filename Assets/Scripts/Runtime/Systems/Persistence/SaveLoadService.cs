using System.Collections.Generic;
using Assets.Scripts.Runtime.Systems.Inventory;
using Assets.Scripts.Runtime.Systems.Persistence.Bindings;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Runtime.Systems.Persistence
{
    public class SaveLoadService : MonoBehaviour 
    {
        [SerializeField] GameData gameData;
        IDataService dataService;

        public GameData GameData => gameData;

        void Awake()
        {
            DontDestroyOnLoad(this);
            dataService = new FileDataService(new JsonSerializer());
        }

        void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;

        void OnDestroy() => SceneManager.sceneLoaded -= OnSceneLoaded;

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "Menu") return;

            Bind<PlayerPersistence, PlayerData>(gameData.playerData);
            Bind<InventoryHandler, InventoryData>(gameData.inventoryData);
        }

        void Bind<T, TData>(TData data) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
        {
            var entities = FindObjectsByType<T>(FindObjectsSortMode.None);

            T entity = null;
            foreach (var e in entities)
            {
                entity = e;
                break;
            }

            if (entity != null)
            {
                data ??= new TData { Id = entity.Id };
                entity.Bind(data);
            }
        }

        void Bind<T, TData>(List<TData> datas) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
        {
            var entities = FindObjectsByType<T>(FindObjectsSortMode.None);

            foreach (var entity in entities)
            {
                TData data = default;
                foreach (var d in datas)
                {
                    if (d.Id == entity.Id)
                    {
                        data = d;
                        break;
                    }
                }

                if (data == null)
                {
                    data = new TData { Id = entity.Id };
                    datas.Add(data);
                }

                entity.Bind(data);
            }
        }

        public void NewGame()
        {
            gameData = new GameData
            {
                Name = "Demo",
                CurrentLevelName = "Gameplay"
            };
        }

        public void SaveGame() => dataService.Save(gameData);

        public void ReloadGame() => LoadGame(gameData.Name);

        public void LoadGame(string gameName)
        {
            gameData = dataService.Load(gameName);
            SceneManager.LoadScene(gameData.CurrentLevelName);
        }

        public void DeleteGame(string gameName) => dataService.Delete(gameName);
    }
}