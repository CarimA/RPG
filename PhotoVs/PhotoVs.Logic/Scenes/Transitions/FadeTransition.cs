﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.Graphics;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Logic.Scenes.Transitions
{
    public class FadeTransition : ITransition
    {
        private readonly IAssetLoader _assetLoader;
        private readonly Color _fadeColor;
        private readonly float _maxFadeInTime;
        private readonly float _maxFadeOutTime;
        private readonly Renderer _renderer;
        private readonly SpriteBatch _spriteBatch;
        private float _fadeInTime;
        private float _fadeOutTime;
        private bool _hasSwitched;

        public FadeTransition(Services services, Color fadeColor, float fadeInTime = 0.35f,
            float fadeOutTime = 0.35f)
        {
            _renderer = services.Get<Renderer>();
            _assetLoader = services.Get<IAssetLoader>();
            _spriteBatch = services.Get<SpriteBatch>();

            _fadeColor = fadeColor;
            _maxFadeInTime = fadeInTime;
            _fadeInTime = fadeInTime;
            _maxFadeOutTime = fadeOutTime;
            _fadeOutTime = fadeOutTime;
            _hasSwitched = false;
        }

        public bool IsFinished { get; private set; }

        public void Update(GameTime gameTime)
        {
            if (!_hasSwitched)
                _fadeInTime -= gameTime.GetElapsedSeconds();
            else
                _fadeOutTime -= gameTime.GetElapsedSeconds();

            if (_fadeOutTime <= 0f)
                IsFinished = true;
        }

        public void Draw(GameTime gameTime)
        {
            var fadeTexture = _assetLoader.Get<Texture2D>("ui/pixel.png");

            _spriteBatch.Begin();
            _spriteBatch.Draw(fadeTexture, new Rectangle(0, 0, _renderer.VirtualWidth, _renderer.VirtualHeight),
                _hasSwitched
                    ? _fadeColor * (_fadeOutTime / _maxFadeOutTime)
                    : _fadeColor * (1f - _fadeInTime / _maxFadeInTime)
            );
            _spriteBatch.End();
        }

        public bool ShouldSwitch()
        {
            if (!(_fadeInTime <= 0f) || _hasSwitched)
                return false;

            _hasSwitched = true;
            return true;
        }
    }
}