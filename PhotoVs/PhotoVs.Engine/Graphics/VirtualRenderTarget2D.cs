using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhotoVs.Engine.Graphics
{
    public class VirtualRenderTarget2D : RenderTarget2D
    {
        private Rectangle _viewport;

        public VirtualRenderTarget2D(GraphicsDevice graphicsDevice, int width, int height)
            : base(graphicsDevice, width, height)
        {
        }

        public void UpdateViewport(int displayWidth, int displayHeight)
        {
            var widthScale = displayWidth / (double) Width;
            var heightScale = displayHeight / (double) Height;

            if (widthScale < heightScale)
            {
                _viewport.Width = (int) (Width * widthScale);
                _viewport.Height = (int) (Height * widthScale);
            }
            else
            {
                _viewport.Width = (int) (Width * heightScale);
                _viewport.Height = (int) (Height * heightScale);
            }

            _viewport.X = displayWidth / 2 - _viewport.Width / 2;
            _viewport.Y = displayHeight / 2 - _viewport.Height / 2;
        }

        public void DrawScaled(SpriteBatch spriteBatch)
        {
            spriteBatch?.Draw(this, _viewport, Color.White);
        }
    }
}