using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhotoVs.Engine.Graphics.Filters
{

    public class WaterReflectionFilter : IFilter
    {
        private readonly IRenderer _renderer;
        private readonly SpriteBatch _spriteBatch;
        private readonly EffectPass _effectPass;

        private readonly EffectParameter _pixelHeightParam;

        public WaterReflectionFilter(IRenderer renderer, SpriteBatch spriteBatch, Effect effect)
        {
            _renderer = renderer;
            _spriteBatch = spriteBatch;

            _pixelHeightParam = effect.Parameters["pixHeight"];

            var waterColorParam = effect.Parameters["water"];
            waterColorParam.SetValue(
                new Vector4(0.03529411764f, 0.3725490196f, 0.47843137254f, 1.0f));

            _effectPass = effect.CurrentTechnique.Passes[0];
        }

        public void Filter(ref RenderTarget2D renderTarget, SpriteBatch spriteBatch, Texture2D inputTexture)
        {
            _pixelHeightParam.SetValue(1f / renderTarget.Height);

            _renderer.RequestSubRenderer(renderTarget);

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp);
            _effectPass.Apply();
            _spriteBatch.Draw(inputTexture, Vector2.Zero, Color.White);
            _spriteBatch.End();

            _renderer.RelinquishSubRenderer();
        }
    }
}
