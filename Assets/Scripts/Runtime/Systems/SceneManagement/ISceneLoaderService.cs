using UnityEngine;

namespace Assets.Scripts.Runtime.Systems.SceneManagement
{
    public interface ISceneLoaderService
    {
        Awaitable LoadSceneGroup(int index);
    }
}