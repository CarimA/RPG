using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhotoVs.Engine.Graphics
{
    public class ColorGrading
    {
        private readonly Effect _effect;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly Texture2D _lut;
        private VirtualRenderTarget2D _view;

        public ColorGrading(GraphicsDevice graphicsDevice, CanvasSize canvasSize, Effect effect, Texture2D lut)
        {
            _graphicsDevice = graphicsDevice;
            _effect = effect;
            _lut = lut;
            _view = new VirtualRenderTarget2D(graphicsDevice, canvasSize.GetWidth(), canvasSize.GetHeight());
        }

        public VirtualRenderTarget2D Filter(SpriteBatch spriteBatch, Texture2D pass)
        {
            if (_view.Width != pass.Width || _view.Height != pass.Height)
            {
                _view = new VirtualRenderTarget2D(_graphicsDevice, pass.Width, pass.Height);
            }

            _graphicsDevice.SetRenderTarget(_view);
            _graphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);

            _effect.CurrentTechnique.Passes[0].Apply();
            _effect.Parameters["LutTexture"].SetValue(_lut);
            _effect.Parameters["LutWidth"].SetValue((float) _lut.Width);
            _effect.Parameters["LutHeight"].SetValue((float) _lut.Height);
            
            spriteBatch.Draw(pass, Vector2.Zero, Color.White);

            spriteBatch.End();

            _graphicsDevice.SetRenderTarget(null);
            return _view;
        }
    }
}