using System;
using Microsoft.Xna.Framework;
using PhotoVs.Utils.Logging;

namespace PhotoVs.Engine.Audio
{
    public class DebugAudio : IAudio, IDisposable
    {
        private readonly IAudio _instance;

        public DebugAudio(IAudio audio)
        {
            _instance = audio;
        }

        public void PlayBgm(string bgm)
        {
            _instance.PlayBgm(bgm);
            Logger.Write.Info($"Playing background music ({bgm})");
        }

        public void PlaySfx(string sfx)
        {
            _instance.PlaySfx(sfx);
            Logger.Write.Info($"Playing sound effect ({sfx})");
        }

        public void PlaySfx(string sfx, Vector2 audioSource, Vector2 position)
        {
            _instance.PlaySfx(sfx, audioSource, position);
            Logger.Write.Info($"Playing sound effect ({sfx}, source: {audioSource}, listener: {position})");
        }

        public void ResumeBgm()
        {
            _instance.ResumeBgm();
            Logger.Write.Info($"Resuming background music");
        }

        public void PauseBgm()
        {
            _instance.PauseBgm();
            Logger.Write.Info($"Pausing background music");
        }

        public void StopBgm()
        {
            _instance.StopBgm();
            Logger.Write.Info($"Stopping background music");
        }

        public void SetBgmVolume(float volume)
        {
            _instance.SetBgmVolume(volume);
            Logger.Write.Info($"Set background music volume to {volume}");
        }

        public void MuteBgm()
        {
            _instance.MuteBgm();
            Logger.Write.Info($"Muting background music");
        }

        public void UnmuteBgm()
        {
            _instance.UnmuteBgm();
            Logger.Write.Info($"Unmuting background music");
        }

        public void SetSfxVolume(float volume)
        {
            _instance.SetSfxVolume(volume);
            Logger.Write.Info($"Set sound effect volume to {volume}");
        }

        public void MuteSfx()
        {
            _instance.MuteSfx();
            Logger.Write.Info($"Muting sound effect");
        }

        public void UnmuteSfx()
        {
            _instance.UnmuteSfx();
            Logger.Write.Info($"Unmuting sound effect");
        }

        public void Dispose()
        {
            if (_instance is IDisposable disposable)
                disposable.Dispose();
        }
    }
}
