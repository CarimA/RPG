using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Engine.Graphics.Filters
{
    public class FunkyFilter : IUpdateFilter
    {
        private readonly Renderer _renderer;

        private readonly Effect _effect;
        private readonly EffectPass _effectPass;
        private readonly EffectParameter _noiseTextureAParam;
        private readonly EffectParameter _noiseTextureBParam;
        private readonly EffectParameter _offsetAParam;
        private readonly EffectParameter _offsetBParam;
        private readonly EffectParameter _colorAParam;
        private readonly EffectParameter _colorBParam;
        private readonly EffectParameter _colorCParam;
        private readonly EffectParameter _noiseSizeParam;
        private readonly EffectParameter _maskSizeParam;

        private RenderTarget2D _outputTexture;

        private Vector2 _offsetA;
        private Vector2 _offsetB;

        public FunkyFilter(Renderer renderer, Effect effect, Texture2D noiseA, 
            Texture2D noiseB, Color colorA, Color colorB, Color colorC)
        {
            _renderer = renderer;
            _effect = effect;

            _effectPass = _effect.CurrentTechnique.Passes[0];
            _noiseTextureAParam = _effect.Parameters["texNoiseA"];
            _noiseTextureBParam = _effect.Parameters["texNoiseB"];
            _offsetAParam = _effect.Parameters["offsetA"];
            _offsetBParam = _effect.Parameters["offsetB"];
            _colorAParam = _effect.Parameters["colorA"];
            _colorBParam = _effect.Parameters["colorB"];
            _colorCParam = _effect.Parameters["colorC"];
            _noiseSizeParam = _effect.Parameters["noiseSize"];
            _maskSizeParam = _effect.Parameters["maskSize"];

            _noiseTextureAParam.SetValue(noiseA);
            _noiseTextureBParam.SetValue(noiseB);
            _noiseSizeParam.SetValue(new Vector2(noiseA.Width, noiseA.Height));

            _colorAParam.SetValue(colorA.ToVector4());
            _colorBParam.SetValue(colorB.ToVector4());
            _colorCParam.SetValue(colorC.ToVector4());
        }

        public RenderTarget2D Filter(SpriteBatch spriteBatch, Texture2D inputTexture)
        {
            if (_outputTexture == null || _outputTexture.Width != inputTexture.Width ||
                _outputTexture.Height != inputTexture.Height)
            {
                _outputTexture = new RenderTarget2D(_renderer.GraphicsDevice, inputTexture.Width, inputTexture.Height);
                _maskSizeParam.SetValue(new Vector2(_outputTexture.Width, _outputTexture.Height));
            }

            _renderer.RequestSubRenderer(_outputTexture);

            _renderer.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointWrap);
            _effect.Parameters["Texture"].SetValue(inputTexture);
            _effectPass.Apply();
            _renderer.SpriteBatch.Draw(inputTexture, Vector2.Zero, Color.White);
            _renderer.SpriteBatch.End();

            _renderer.RelinquishSubRenderer();
            return _outputTexture;
        }

        public void Update(GameTime gameTime)
        {
            _offsetA += new Vector2(0.015f * gameTime.GetElapsedSeconds());
            _offsetAParam.SetValue(_offsetA);

            _offsetB -= new Vector2(0.015f * gameTime.GetElapsedSeconds());
            _offsetBParam.SetValue(_offsetB);
        }
    }
}
