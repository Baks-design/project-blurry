using Assets.Scripts.Runtime.Utilities.Patterns.ServicesLocator;
using KBCore.Refs;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Runtime.Systems.SceneManagement
{
    public class SceneLoaderService : MonoBehaviour, ISceneLoaderService
    {
        public readonly SceneGroupManager manager = new();

        [SerializeField, Anywhere] Image loadingBar;
        [SerializeField, Child] Canvas loadingCanvas;
        [SerializeField, Child] CinemachineCamera loadingCamera;
        [SerializeField] float fillSpeed = 0.5f;
        [SerializeField] SceneGroup[] sceneGroups;
        float targetProgress;
        bool isLoading;

        void Awake()
        {
            DontDestroyOnLoad(this);
            ServiceLocator.Global.Register<ISceneLoaderService>(this);
        }

        async void Start() => await LoadSceneGroup(0);

        void Update()
        {
            if (!isLoading) return;

            var currentFillAmount = loadingBar.fillAmount;
            var progressDifference = Mathf.Abs(currentFillAmount - targetProgress);

            var dynamicFillSpeed = progressDifference * fillSpeed;

            loadingBar.fillAmount = Mathf.Lerp(currentFillAmount, targetProgress, Time.deltaTime * dynamicFillSpeed);
        }

        public async Awaitable LoadSceneGroup(int index)
        {
            loadingBar.fillAmount = 0f;
            targetProgress = 1f;

            if (index < 0 || index >= sceneGroups.Length)
            {
                Debug.LogError($"Invalid scene group index: {index}");
                return;
            }

            var progress = new LoadingProgress();
            progress.OnProgressed += target => targetProgress = Mathf.Max(target, targetProgress);

            EnableLoadingCanvas();
            await manager.LoadScenes(sceneGroups[index], progress);
            EnableLoadingCanvas(false);
        }

        void EnableLoadingCanvas(bool enable = true)
        {
            isLoading = enable;
            loadingCanvas.gameObject.SetActive(enable);
            loadingCamera.gameObject.SetActive(enable);
        }
    }
}