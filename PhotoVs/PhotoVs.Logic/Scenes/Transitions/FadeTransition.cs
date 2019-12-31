﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Logic.Services;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Logic.Scenes.Transitions
{
    public class FadeTransition : ITransition
    {
        public bool IsFinished { get; private set; }

        private ServiceLocator _services;

        private Color _fadeColor;
        private float _maxFadeInTime;
        private float _fadeInTime;
        private float _maxFadeOutTime;
        private float _fadeOutTime;
        private bool _hasSwitched;

        public FadeTransition(ServiceLocator services, Color fadeColor, float fadeInTime = 0.35f, float fadeOutTime = 0.35f)
        {
            _services = services;
            _fadeColor = fadeColor;
            _maxFadeInTime = fadeInTime;
            _fadeInTime = fadeInTime;
            _maxFadeOutTime = fadeOutTime;
            _fadeOutTime = fadeOutTime;
            _hasSwitched = false;
        }

        public void Update(GameTime gameTime)
        {
            if (!_hasSwitched)
            {
                _fadeInTime -= gameTime.GetElapsedSeconds();
            }
            else
            {
                _fadeOutTime -= gameTime.GetElapsedSeconds();
            }

            if (_fadeOutTime <= 0f)
            {
                IsFinished = true;
            }
        }

        public void Draw(GameTime gameTime)
        {
            var spriteBatch = _services.SpriteBatch;
            var assetLoader = _services.AssetLoader;
            var canvasSize = _services.Renderer.CanvasSize;
            var fadeTexture = assetLoader.GetAsset<Texture2D>("interfaces/black.png");

            spriteBatch.Begin();
            spriteBatch.Draw(fadeTexture, new Rectangle(0, 0, canvasSize.GetWidth(), canvasSize.GetHeight()),
                _hasSwitched
                    ? _fadeColor * (_fadeOutTime / _maxFadeOutTime)
                    : _fadeColor * (1f - (_fadeInTime / _maxFadeInTime))
                    );
            spriteBatch.End();
        }

        public bool ShouldSwitch()
        {
            if (!(_fadeInTime <= 0f) || _hasSwitched) return false;

            _hasSwitched = true;
            return true;

        }
    }
}
