using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Graphics;
using PhotoVs.Engine.Graphics.Filters;
using PhotoVs.Logic.Mechanics;
using PhotoVs.Logic.Mechanics.World;
using PhotoVs.Utils.Collections;

namespace PhotoVs.Logic.Filters
{
    public class DayNightFilter : IFilter
    {
        private readonly IRenderer _renderer;
        private readonly SpriteBatch _spriteBatch;
        private readonly GameDate _date;
        private ColorAverager _averager;
        private ColorGradingFilter _colorGrading;
        private LinearTweener<Texture2D> _tweener;

        public DayNightFilter(IRenderer renderer, SpriteBatch spriteBatch, GameDate date, Effect averageEffect, Effect colorGradingEffect, List<(float, Texture2D)> _luts)
        {
            _renderer = renderer;
            _spriteBatch = spriteBatch;
            _date = date;
            _averager = new ColorAverager(renderer, averageEffect);
            _colorGrading = new ColorGradingFilter(renderer, spriteBatch, colorGradingEffect);
            _tweener = new LinearTweener<Texture2D>();

            foreach (var (time, texture) in _luts)
                _tweener.AddPoint(time, texture);
        }

        public void Filter(ref RenderTarget2D renderTarget, SpriteBatch spriteBatch, Texture2D inputTexture)
        {
            if (!_date.TimeFlowing)
            {
                _renderer.RequestSubRenderer(renderTarget);
                _spriteBatch.Begin();
                _spriteBatch.Draw(inputTexture, Vector2.Zero, Color.White);
                _spriteBatch.End();
                _renderer.RelinquishSubRenderer();
                
                return;
            }

            var (phase, texA, texB) = _tweener.GetPhase(_date.TimeScale);
            _averager.SetTextures(texA, texB);
            _averager.SetPhase(phase);

            _colorGrading.LookupTable = _averager.Average(spriteBatch);
            _colorGrading.Filter(ref renderTarget, spriteBatch, inputTexture);
        }
    }
}
