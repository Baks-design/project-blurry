namespace Assets.Scripts.Runtime.Systems.Audio
{
    public interface ISoundService
    {
        void ReturnToPool(SoundEmitter soundEmitter);
        SoundBuilder CreateSoundBuilder();
    }
}