using System;
using System.Collections.ObjectModel;
using System.Linq;
using CSCore;
using CSCore.Codecs;
using CSCore.Codecs.OGG;
using CSCore.CoreAudioAPI;
using CSCore.SoundOut;
using Microsoft.Xna.Framework;

namespace PhotoVs.Engine.Audio
{
    public class CscoreAudio : IAudio, IDisposable
    {
        private event EventHandler<PlaybackStoppedEventArgs> PlaybackStopped;

        private readonly ObservableCollection<MMDevice> _devices = new ObservableCollection<MMDevice>();
        private ISoundOut _soundOut;
        private IWaveSource _waveSource;

        private PlaybackState PlaybackState => _soundOut?.PlaybackState ?? PlaybackState.Stopped;

        private TimeSpan Position
        {
            get => _waveSource?.GetPosition() ?? TimeSpan.Zero;
            set => _waveSource?.SetPosition(value);
        }

        private TimeSpan Length => _waveSource?.GetLength() ?? TimeSpan.Zero;

        private int Volume
        {
            get => _soundOut != null ? Math.Min(100, Math.Max((int)(_soundOut.Volume * 100), 0)) : 100;
            set
            {
                if (_soundOut != null)
                    _soundOut.Volume = Math.Min(1.0f, Math.Max(value / 100f, 0f));
            }
        }

        public CscoreAudio()
        {
            CodecFactory.Instance.Register("ogg-vorbis",
                new CodecFactoryEntry(s => new OggSource(s).ToWaveSource(), ".ogg"));

            //Logger.Debug(CodecFactory.SupportedFilesFilterEn);

            using var mmDeviceEnumerator = new MMDeviceEnumerator();
            using var mmDeviceCollection = mmDeviceEnumerator.EnumAudioEndpoints(DataFlow.Render, DeviceState.Active);
            _devices.Add(mmDeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console));
            //Logger.Info("Default Audio Device: {0}", _devices.First().FriendlyName);
            foreach (var device in mmDeviceCollection)
            {
                _devices.Add(device);
                //Logger.Debug("Available Audio Device: {0}", device.FriendlyName);
            }
        }

        public void PlayBgm(string bgm)
        {
            Open(bgm, _devices.First());
            Play();
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

        private void Open(string filename, MMDevice device)
        {
            CleanupPlayback();

            _waveSource =
                CodecFactory.Instance.GetCodec($"assets/audio/bgm/{filename}")
                    .Loop()
                    .ToSampleSource()
                    .ToWaveSource();
            _soundOut = new WasapiOut { Latency = 100, Device = device };
            _soundOut.Initialize(_waveSource);

            if (PlaybackStopped != null)
                _soundOut.Stopped += PlaybackStopped;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            CleanupPlayback();
        }

        private void Play()
        {
            _soundOut?.Play();
        }

        private void Pause()
        {
            _soundOut?.Pause();
        }

        private void Stop()
        {
            _soundOut?.Stop();
        }

        private void CleanupPlayback()
        {
            if (_soundOut != null)
            {
                _soundOut.Dispose();
                _soundOut = null;
            }

            if (_waveSource != null)
            {
                _waveSource.Dispose();
                _waveSource = null;
            }
        }
    }
}
