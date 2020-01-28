using Microsoft.Xna.Framework;

namespace PhotoVs.Models.Audio
{
    public interface IAudio
    {
        void PlayBgm(string bgm);
        void PlaySfx(string sfx);
        void PlaySfx(string sfx, Vector2 audioSource, Vector2 position);

        void ResumeBgm();
        void PauseBgm();
        void StopBgm();

        void SetBgmVolume(float volume);
        void MuteBgm();
        void UnmuteBgm();

        void SetSfxVolume(float volume);
        void MuteSfx();
        void UnmuteSfx();
    }
}