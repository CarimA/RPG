using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhotoVs.Engine.Graphics
{
    public class ColorAverager
    {
        private readonly Renderer _renderer;
        private readonly Effect _effect;
        private readonly GraphicsDevice _graphicsDevice;
        private Texture2D _texA;
        private Texture2D _texB;
        private RenderTarget2D _outputTex;
        private float _phase;

        public void Set(float phase, Texture2D texA, Texture2D texB)
        {
            _phase = phase;
            _texA = texA;
            _texB = texB;
            if (_outputTex == null || (_outputTex.Width != texA.Width || _outputTex.Height != texA.Height)
                                   || (_outputTex.Width != texB.Width || _outputTex.Height != texB.Height))
            {
                _outputTex = new RenderTarget2D(_graphicsDevice, texA.Width, texA.Height);
            }
        }

        public ColorAverager(Renderer renderer, Effect effect)
        {
            _renderer = renderer;
            _graphicsDevice = renderer.GraphicsDevice;
            _effect = effect;
        }

        public RenderTarget2D Average(SpriteBatch spriteBatch)
        {
            _renderer.RequestSubRenderer(_outputTex);

            spriteBatch.Begin(SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);

            _effect.CurrentTechnique.Passes[0].Apply();
            _effect.Parameters["texA"].SetValue(_texA);
            _effect.Parameters["texB"].SetValue(_texB);
            _effect.Parameters["phase"].SetValue(_phase);

            spriteBatch.Draw(_texA, Vector2.Zero, Color.White);

            spriteBatch.End();

            _renderer.RelinquishSubRenderer();
            return _outputTex;
        }
    }
}