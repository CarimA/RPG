using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhotoVs.Engine.Graphics.Filters
{
    public class ColorGradingFilter : IDisposable, IFilter
    {
        private readonly NewRenderer _renderer;

        private readonly Effect _effect;
        private readonly EffectParameter _lutTextureParam;
        private readonly EffectParameter _lutTextureWidthParam;
        private readonly EffectParameter _lutTextureHeightParam;
        private readonly EffectPass _effectPass;

        private Texture2D _lookupTable;

        public Texture2D LookupTable
        {
            get => _lookupTable;
            set
            {
                if (value == _lookupTable)
                    return;
                
                _lookupTable = value;
                _lutTextureParam.SetValue(value);
                _lutTextureWidthParam.SetValue((float)value.Width);
                _lutTextureHeightParam.SetValue((float)value.Height);
            }
        }

        public ColorGradingFilter(NewRenderer renderer, Effect effect)
        {
            _renderer = renderer;

            _effect = effect;
            _lutTextureParam = _effect.Parameters["LutTexture"];
            _lutTextureWidthParam = _effect.Parameters["LutWidth"];
            _lutTextureHeightParam = _effect.Parameters["LutHeight"];
            _effectPass = _effect.CurrentTechnique.Passes[0];
        }

        public RenderTarget2D Filter(SpriteBatch spriteBatch, Texture2D inputTexture)
        {
            _renderer.RequestSubRenderer(inputTexture.Width, inputTexture.Height);

            _renderer.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp);
            _effectPass.Apply();
            _renderer.SpriteBatch.Draw(inputTexture, Vector2.Zero, Color.White);
            _renderer.SpriteBatch.End();

            return _renderer.RelinquishSubRenderer();
        }

        public void Dispose()
        {
            _effect?.Dispose();;
        }
    }
}
