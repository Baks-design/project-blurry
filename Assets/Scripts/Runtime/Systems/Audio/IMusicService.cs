using UnityEngine;

namespace Assets.Scripts.Runtime.Systems.Audio
{
    public interface IMusicService
    {
        void AddToPlaylist(AudioClip clip);
        void Clear();
        void PlayNextTrack();
        void Play(AudioClip clip);
    }
}