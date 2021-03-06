﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Engine.Graphics.Filters
{
    public class FunkyFilter : IUpdateFilter
    {
        private readonly EffectParameter _colorAParam;
        private readonly EffectParameter _colorBParam;
        private readonly EffectParameter _colorCParam;

        private readonly Effect _effect;
        private readonly EffectPass _effectPass;
        private readonly EffectParameter _maskSizeParam;
        private readonly EffectParameter _noiseSizeParam;
        private readonly EffectParameter _noiseTextureAParam;
        private readonly EffectParameter _noiseTextureBParam;
        private readonly EffectParameter _offsetAParam;
        private readonly EffectParameter _offsetBParam;
        private readonly EffectParameter _pulseParam;
        private readonly IRenderer _renderer;
        private readonly SpriteBatch _spriteBatch;

        private Vector2 _offsetA;
        private Vector2 _offsetB;

        public FunkyFilter(IRenderer renderer, SpriteBatch spriteBatch, Effect effect, Texture2D noiseA,
            Texture2D noiseB, Color colorA, Color colorB, Color colorC)
        {
            _renderer = renderer;
            _spriteBatch = spriteBatch;
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
            _pulseParam = _effect.Parameters["pulses"];

            _noiseTextureAParam.SetValue(noiseA);
            _noiseTextureBParam.SetValue(noiseB);
            _noiseSizeParam.SetValue(new Vector2(noiseA.Width, noiseA.Height));

            _colorAParam.SetValue(colorA.ToVector4());
            _colorBParam.SetValue(colorB.ToVector4());
            _colorCParam.SetValue(colorC.ToVector4());

            _pulseParam.SetValue(6f);
        }

        public void Filter(ref RenderTarget2D renderTarget, SpriteBatch spriteBatch, Texture2D inputTexture)
        {
            _maskSizeParam.SetValue(new Vector2(renderTarget.Width, renderTarget.Height));

            _renderer.RequestSubRenderer(renderTarget);

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointWrap);
            _effect.Parameters["Texture"].SetValue(inputTexture);
            _effectPass.Apply();
            _spriteBatch.Draw(inputTexture, Vector2.Zero, Color.White);
            _spriteBatch.End();

            _renderer.RelinquishSubRenderer();
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