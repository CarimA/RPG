using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhotoVs.Graphics
{
    public class ColorGrading
    {
        private readonly CanvasSize _canvasSize;

        private readonly Effect _effect;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly Texture2D _lut;
        private readonly VirtualRenderTarget2D _view;

        public ColorGrading(GraphicsDevice graphicsDevice, CanvasSize canvasSize, Effect effect, Texture2D lut)
        {
            _graphicsDevice = graphicsDevice;
            _effect = effect;
            _lut = lut;
            _canvasSize = canvasSize;
            _view = new VirtualRenderTarget2D(graphicsDevice, canvasSize.GetWidth(), canvasSize.GetHeight());
        }

        public VirtualRenderTarget2D Filter(SpriteBatch spriteBatch, Texture2D pass)
        {
            _graphicsDevice.SetRenderTarget(_view);
            _graphicsDevice.Clear(Color.Black);


            spriteBatch.Begin(SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);

            _effect.CurrentTechnique.Passes[0].Apply();
            _effect.Parameters["palette"].SetValue(_lut);
            _effect.Parameters["tex_width"].SetValue((float) _lut.Width);
            _effect.Parameters["tex_height"].SetValue((float) _lut.Height);

            spriteBatch.Draw(pass, Vector2.Zero, Color.White);

            spriteBatch.End();

            _graphicsDevice.SetRenderTarget(null);
            return _view;
        }
    }
}