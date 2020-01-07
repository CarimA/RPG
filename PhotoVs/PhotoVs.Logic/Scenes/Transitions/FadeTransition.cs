using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Logic.Scenes.Transitions
{
    public class FadeTransition : ITransition
    {
        private readonly Color _fadeColor;
        private float _fadeInTime;
        private float _fadeOutTime;
        private bool _hasSwitched;
        private readonly float _maxFadeInTime;
        private readonly float _maxFadeOutTime;

        private readonly Services _services;

        public FadeTransition(Services services, Color fadeColor, float fadeInTime = 0.35f,
            float fadeOutTime = 0.35f)
        {
            _services = services;
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

            if (_fadeOutTime <= 0f) IsFinished = true;
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
                    : _fadeColor * (1f - _fadeInTime / _maxFadeInTime)
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