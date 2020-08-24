using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhotoVs.Engine.Graphics.Filters
{
    public class BgraToRgbaFilter : IDisposable, IFilter
    {
        private readonly Effect _effect;
        private readonly EffectPass _effectPass;
        private readonly IRenderer _renderer;
        private readonly SpriteBatch _spriteBatch;

        public BgraToRgbaFilter(IRenderer renderer, SpriteBatch spriteBatch, Effect effect)
        {
            _renderer = renderer;
            _spriteBatch = spriteBatch;

            _effect = effect;
            _effectPass = _effect.CurrentTechnique.Passes[0];
        }

        public void Dispose()
        {
            _effect?.Dispose();
        }

        public void Filter(ref RenderTarget2D renderTarget, SpriteBatch spriteBatch, Texture2D inputTexture)
        {
            _renderer.RequestSubRenderer(renderTarget);

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp);
            _effectPass.Apply();
            _spriteBatch.Draw(inputTexture, Vector2.Zero, Color.White);
            _spriteBatch.End();

            _renderer.RelinquishSubRenderer();
        }
    }
}