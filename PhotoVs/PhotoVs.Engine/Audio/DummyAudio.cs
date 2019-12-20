using System;
using Microsoft.Xna.Framework;
using PhotoVs.Models.Audio;

namespace PhotoVs.Engine.Audio
{
    public class DummyAudio : IAudio
    {
        public void PlayBgm(string bgm)
        {
            throw new NotImplementedException();
        }

        public void PlaySfx(string sfx)
        {
            throw new NotImplementedException();
        }

        public void PlaySfx(string sfx, Vector2 audioSource, Vector2 position)
        {
            throw new NotImplementedException();
        }

        public void ResumeBgm()
        {
            throw new NotImplementedException();
        }

        public void PauseBgm()
        {
            throw new NotImplementedException();
        }

        public void StopBgm()
        {
            throw new NotImplementedException();
        }

        public void SetBgmVolume(float volume)
        {
            throw new NotImplementedException();
        }

        public void MuteBgm()
        {
            throw new NotImplementedException();
        }

        public void UnmuteBgm()
        {
            throw new NotImplementedException();
        }

        public void SetSfxVolume(float volume)
        {
            throw new NotImplementedException();
        }

        public void MuteSfx()
        {
            throw new NotImplementedException();
        }

        public void UnmuteSfx()
        {
            throw new NotImplementedException();
        }
    }
}