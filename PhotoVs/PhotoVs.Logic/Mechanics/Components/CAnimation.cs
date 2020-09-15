using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Logic.Mechanics.Components
{
    public class CAnimation
    {
        private Dictionary<string, List<AnimationFrame>> _frames;
        private string _activeAnimation;
        private string _defaultAnimation;
        private int _frame;
        private bool _isPaused;
        private float _time;

        public bool Loop { get; set; }
        public Action<string> OnComplete { get; set; }

        public CAnimation()
        {
            _frames = new Dictionary<string, List<AnimationFrame>>();
            _isPaused = true;
        }

        public void AddAnimation(string id, List<AnimationFrame> frames)
        {
            _frames.Add(id, frames);
        }

        public void SetDefaultAnimation(string id)
        {
            _defaultAnimation = id;
        }

        public void Play(string id)
        {
            if (_activeAnimation != string.Empty &&_activeAnimation == id)
               return;

            _activeAnimation = id;
            _frame = 0;
            _isPaused = false;
        }

        public void Pause()
        {
            _isPaused = true;
        }

        public void Stop()
        {
            _activeAnimation = string.Empty;
            _isPaused = true;
            _frame = 0;
        }

        public void Update(GameTime gameTime)
        {
            if (_isPaused || _activeAnimation == string.Empty || _frames[_activeAnimation].Count == 1)
                return;

            if (_time <= 0)
            {
                _frame++;
                if (_frame >= _frames[_activeAnimation].Count)
                {
                    OnComplete?.Invoke(_activeAnimation);

                    if (Loop)
                        _frame = 0;
                    else
                        Stop();
                }
                else
                {
                    _time = _frames[_activeAnimation][_frame].Duration;
                }
            }

            _time -= gameTime.GetElapsedSeconds();
        }

        public Rectangle GetFrame()
        {
            if (_activeAnimation == string.Empty)
                return Rectangle.Empty;

            return _frames[_activeAnimation][_frame].Source;
        }
    }
}
