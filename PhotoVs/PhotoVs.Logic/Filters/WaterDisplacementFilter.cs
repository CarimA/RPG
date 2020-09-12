using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Graphics;
using PhotoVs.Engine.Graphics.Filters;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Logic.Filters
{

    public class WaterDisplacementFilter : IUpdateFilter
    {
        private readonly IRenderer _renderer;
        private readonly SpriteBatch _spriteBatch;
        private readonly VirtualResolution _virtualResolution;
        private readonly EffectPass _effectPass;

        private readonly EffectParameter _textureParam;
        private EffectParameter _offsetParam;
        private EffectParameter _pixelWidthParam;
        private EffectParameter _pixelHeightParam;

        private Vector2 _offset;

        private float throttleTime;

        public WaterDisplacementFilter(IRenderer renderer, SpriteBatch spriteBatch, VirtualResolution virtualResolution, Effect effect, Texture2D displaceA, Texture2D displaceB)
        {
            _renderer = renderer;
            _spriteBatch = spriteBatch;
            _virtualResolution = virtualResolution;

            _textureParam = effect.Parameters["Texture"];
            _offsetParam = effect.Parameters["offset"];
            _pixelWidthParam = effect.Parameters["pixWidth"];
            _pixelHeightParam = effect.Parameters["pixHeight"];

            effect.Parameters["texDisplace"].SetValue(displaceA);
            effect.Parameters["texDisplace2"].SetValue(displaceB);
            effect.Parameters["maxDisplace"].SetValue(8f);
            effect.Parameters["water"].SetValue(new Vector4(0.01568628F, 0.172549F, 0.2235294F, 1.0f));

            _effectPass = effect.CurrentTechnique.Passes[0];
        }

        public void SetTextures(Texture2D texture)
        {
            _textureParam.SetValue(texture);
        }

        public void Update(GameTime gameTime)
        {
            //_cameraPosParam.SetValue(_camera.Position);
            //_scaleParam.SetValue(_camera.InverseZoom * 2);

            throttleTime -= gameTime.GetElapsedSeconds();
            if (throttleTime <= 0)
            {
                throttleTime = 1f / 15;
            }
            else
            {
                return;
            }

            _offset -= new Vector2(7.95f, 19.2f) * throttleTime / 600f;
            _offsetParam.SetValue(_offset);
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
