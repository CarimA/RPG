using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhotoVs.Engine.Graphics
{
    public class ColorAverager
    {
        private readonly Effect _effect;
        private readonly EffectParameter _phaseParameter;
        private readonly IRenderer _renderer;
        private readonly EffectParameter _texAParameter;
        private readonly EffectParameter _texBParameter;
        private RenderTarget2D _outputTex;
        private Texture2D _texA;
        private Texture2D _texB;

        public ColorAverager(IRenderer renderer, Effect effect)
        {
            _renderer = renderer;
            _effect = effect;

            _texAParameter = _effect.Parameters["texA"];
            _texBParameter = _effect.Parameters["texB"];
            _phaseParameter = _effect.Parameters["phase"];
        }

        public void SetTextures(Texture2D texA, Texture2D texB)
        {
            if (_texA != texA)
            {
                _texA = texA;
                _texAParameter.SetValue(_texA);
            }

            if (_texB != texB)
            {
                _texB = texB;
                _texBParameter.SetValue(_texB);
            }

            if (_outputTex == null || _outputTex.Width != texA.Width || _outputTex.Height != texA.Height ||
                _outputTex.Width != texB.Width ||
                _outputTex.Height != texB.Height) _outputTex = _renderer.CreateRenderTarget(texA.Width, texA.Height);
        }

        public void SetPhase(float phase)
        {
            _phaseParameter.SetValue(phase);
        }

        public RenderTarget2D Average(SpriteBatch spriteBatch)
        {
            _renderer.RequestSubRenderer(_outputTex);

            spriteBatch.Begin(SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);

            _effect.CurrentTechnique.Passes[0].Apply();

            spriteBatch.Draw(_texA, Vector2.Zero, Color.White);

            spriteBatch.End();

            _renderer.RelinquishSubRenderer();
            return _outputTex;
        }
    }
}