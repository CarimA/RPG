using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Graphics;
using PhotoVs.Engine.Graphics.Filters;
using PhotoVs.Logic.Mechanics;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Logic.Filters
{

    public class WaterFilter : IUpdateFilter
    {
        private readonly IRenderer _renderer;
        private readonly SpriteBatch _spriteBatch;
        private readonly Camera _camera;
        private readonly VirtualResolution _virtualResolution;
        private readonly EffectPass _effectPass;

        private readonly EffectParameter _scaleParam;
        private readonly EffectParameter _pixelWidthParam;
        private readonly EffectParameter _pixelHeightParam;
        private readonly EffectParameter _offsetAParam;
        private readonly EffectParameter _offsetBParam;
        private readonly EffectParameter _cameraPosParam;

        private Vector2 _offsetA;
        private Vector2 _offsetB;

        private float throttleTime;

        public WaterFilter(IRenderer renderer, SpriteBatch spriteBatch, Camera camera, Effect effect, Texture2D noiseA, Texture2D noiseB, VirtualResolution virtualResolution)
        {
            _renderer = renderer;
            _spriteBatch = spriteBatch;
            _camera = camera;
            _virtualResolution = virtualResolution;

            _scaleParam = effect.Parameters["scale"];
            _pixelWidthParam = effect.Parameters["pixWidth"];
            _pixelHeightParam = effect.Parameters["pixHeight"];
            _offsetAParam = effect.Parameters["offsetA"];
            _offsetBParam = effect.Parameters["offsetB"];
            _cameraPosParam = effect.Parameters["cameraPos"];

            var noiseAParam = effect.Parameters["texNoiseA"];
            noiseAParam.SetValue(noiseA);

            var noiseBParam = effect.Parameters["texNoiseB"];
            noiseBParam.SetValue(noiseB);

            var contrastParam = effect.Parameters["contrast"];
            contrastParam.SetValue(0.75f);

            var stepParam = effect.Parameters["step"];
            stepParam.SetValue(6);

            var waterColorParam = effect.Parameters["water"];
            waterColorParam.SetValue(new Vector4(0.03529411764f, 0.3725490196f, 0.47843137254f, 1.0f));

            var highlightWaterParam = effect.Parameters["highlightWater"];
            highlightWaterParam.SetValue(new Vector4(0.37647058823f, 0.70588235294f, 0.84705882352f, 1.0f));

            _effectPass = effect.CurrentTechnique.Passes[0];
        }

        public void Update(GameTime gameTime)
        {
            _cameraPosParam.SetValue(_camera.Position);
            _scaleParam.SetValue(_camera.InverseZoom * 2);

            throttleTime -= gameTime.GetElapsedSeconds();
            if (throttleTime <= 0)
            {
                throttleTime = 1f / 15;
            }
            else
            {
                return;
            }

            _offsetA -= new Vector2(.53f, 2.40f) * throttleTime / 300f;
            _offsetB -= new Vector2(.12f, -4.2f) * throttleTime / 300f;

            _offsetAParam.SetValue(_offsetA);
            _offsetBParam.SetValue(_offsetB);
        }

        public void Filter(ref RenderTarget2D renderTarget, SpriteBatch spriteBatch, Texture2D inputTexture)
        {
            _pixelWidthParam.SetValue(1f / _virtualResolution.MaxWidth);
            _pixelHeightParam.SetValue(1f / _virtualResolution.MaxHeight);

            _renderer.RequestSubRenderer(renderTarget);

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp);
            _effectPass.Apply();
            _spriteBatch.Draw(inputTexture, Vector2.Zero, Color.White);
            _spriteBatch.End();

            _renderer.RelinquishSubRenderer();
        }
    }
}
