using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Logic.Mechanics.Components
{
    public class AnimationFrame
    {
        public float Duration;
        public Rectangle Source;

        public AnimationFrame(Rectangle source, float duration)
        {
            Source = source;
            Duration = duration;
        }
    }

    public class CAnimation
    {
        private Dictionary<string, List<AnimationFrame>> _frames;
        private string _activeAnimation;
        private string _defaultAnimation;
        private int _frame;
        private bool _isPaused;
        private float _time;

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
            if (_activeAnimation == id)
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
            _activeAnimation = _defaultAnimation;
            _frame = 0;
        }

        public void Update(GameTime gameTime)
        {
            if (_isPaused)
                return;

            if (_time <= 0)
            {
                _frame++;
                if (_frame >= _frames[_activeAnimation].Count)
                    _frame = 0;

                _time = _frames[_activeAnimation][_frame].Duration;
            }

            _time -= gameTime.GetElapsedSeconds();
        }

        public Rectangle GetFrame()
        {
            return _frames[_activeAnimation][_frame].Source;
        }
    }
}
