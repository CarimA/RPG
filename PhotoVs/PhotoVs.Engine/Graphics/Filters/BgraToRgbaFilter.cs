using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhotoVs.Engine.Graphics.Filters
{
    public class ColorGradingFilter : IDisposable, IFilter
    {
        private readonly Effect _effect;
        private readonly EffectPass _effectPass;
        private readonly EffectParameter _lutTextureHeightParam;
        private readonly EffectParameter _lutTextureParam;
        private readonly EffectParameter _lutTextureWidthParam;
        private readonly IRenderer _renderer;
        private readonly SpriteBatch _spriteBatch;

        private Texture2D _lookupTable;

        public ColorGradingFilter(IRenderer renderer, SpriteBatch spriteBatch, Effect effect)
        {
            _renderer = renderer;
            _spriteBatch = spriteBatch;

            _effect = effect;
            _lutTextureParam = _effect.Parameters["LutTexture"];
            _lutTextureWidthParam = _effect.Parameters["LutWidth"];
            _lutTextureHeightParam = _effect.Parameters["LutHeight"];
            _effectPass = _effect.CurrentTechnique.Passes[0];
        }

        public Texture2D LookupTable
        {
            get => _lookupTable;
            set
            {
                if (value == _lookupTable)
                    return;

                _lookupTable = value;
                _lutTextureParam.SetValue(value);
                _lutTextureWidthParam.SetValue((float) value.Width);
                _lutTextureHeightParam.SetValue((float) value.Height);
            }
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