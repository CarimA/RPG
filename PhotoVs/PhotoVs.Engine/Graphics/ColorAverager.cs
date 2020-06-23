using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhotoVs.Engine.Graphics
{
    public class ColorAverager
    {
        private readonly Effect _effect;
        private readonly GraphicsDevice _graphicsDevice;
        private Texture2D _texA;
        private Texture2D _texB;
        private RenderTarget2D _outputTex;
        private float _phase;

        public RenderTarget2D GetOutputTexture()
        {
            return _outputTex;
        }

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

        public ColorAverager(GraphicsDevice graphicsDevice, Effect effect)
        {
            _graphicsDevice = graphicsDevice;
            _effect = effect;
        }

        public void Average(SpriteBatch spriteBatch)
        {
            _graphicsDevice.SetRenderTarget(_outputTex);
            _graphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);

            _effect.CurrentTechnique.Passes[0].Apply();
            _effect.Parameters["texA"].SetValue(_texA);
            _effect.Parameters["texB"].SetValue(_texB);
            _effect.Parameters["phase"].SetValue(_phase);

            spriteBatch.Draw(_texA, Vector2.Zero, Color.White);

            spriteBatch.End();

            _graphicsDevice.SetRenderTarget(null);
        }
    }
}