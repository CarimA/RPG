using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace PhotoVs.Engine.Graphics
{
    public class Renderer
    {
        public GraphicsDevice GraphicsDevice { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }
        private readonly GraphicsDeviceManager _graphics;
        private readonly GameWindow _window;

        public int VirtualWidth { get; private set; }
        public int VirtualHeight { get; private set; }
        public int VirtualMaxHeight { get; private set; }
        public int VirtualMaxWidth { get; private set; }
        public int GameWidth { get; private set; }
        public int GameHeight { get; private set; }

        private Rectangle _display;

        private RenderTarget2D _mainRenderTarget;
        private RenderTarget2D _tempRenderTarget;

        public Renderer(Services services, int virtualWidth, int virtualHeight, int virtualMaxWidth, int virtualMaxHeight)
        {
            GraphicsDevice = services.Get<GraphicsDevice>();
            SpriteBatch = services.Get<SpriteBatch>();
            _graphics = services.Get<GraphicsDeviceManager>();
            _window = services.Get<GameWindow>();
            VirtualWidth = virtualWidth;
            VirtualHeight = virtualHeight;
            VirtualMaxWidth = virtualMaxWidth;
            VirtualMaxHeight = virtualMaxHeight;

            _display = new Rectangle();
            UpdateDisplay(null, null);
        }

        public void Draw()
        {
            var width = _graphics.PreferredBackBufferWidth;
            var height = _graphics.PreferredBackBufferHeight;

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);
            SpriteBatch.Begin(SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            SpriteBatch.Draw(_mainRenderTarget, _display, Color.White);
            SpriteBatch.End();
        }

        public void BeforeDraw()
        {
            GraphicsDevice.SetRenderTarget(_mainRenderTarget);
            GraphicsDevice.Clear(Color.Magenta);
        }

        public void RequestSubRenderer(RenderTarget2D renderTarget)
        {
            // copy the existing buffer to a temporary buffer so
            // that it isn't wiped
            GraphicsDevice.SetRenderTarget(_tempRenderTarget);
            GraphicsDevice.Clear(Color.Transparent);
            SpriteBatch.Begin();
            SpriteBatch.Draw(_mainRenderTarget, Vector2.Zero, Color.White);
            SpriteBatch.End();

            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(Color.Transparent);
        }

        public RenderTarget2D CreateRenderTarget(int width, int height)
        {
            return new RenderTarget2D(GraphicsDevice, width, height);
        }

        public void RelinquishSubRenderer()
        {
            GraphicsDevice.SetRenderTarget(_mainRenderTarget);
            GraphicsDevice.Clear(Color.Black);
            SpriteBatch.Begin();
            SpriteBatch.Draw(_tempRenderTarget, Vector2.Zero, Color.White);
            SpriteBatch.End();
        }

        private void UpdateDisplay(object sender, EventArgs e)
        {
            _window.ClientSizeChanged -= UpdateDisplay;

            var width = _window.ClientBounds.Width;
            var height = _window.ClientBounds.Height;

            if (width == 0 || height == 0)
            {
                _window.ClientSizeChanged += UpdateDisplay;
                return;
            }

            _graphics.PreferredBackBufferWidth = width;
            _graphics.PreferredBackBufferHeight = height;
            _graphics.ApplyChanges();

            var widthScale = width / (double)VirtualWidth;
            var heightScale = height / (double)VirtualHeight;


            if (widthScale < heightScale)
            {
                GameWidth = VirtualWidth;
                GameHeight = (int)(height / widthScale);
            }
            else
            {
                GameWidth = (int)(width / heightScale);
                GameHeight = VirtualHeight;
            }

            GameWidth = Math.Min(VirtualMaxWidth, GameWidth);
            GameHeight = Math.Min(VirtualMaxHeight, GameHeight);

            _mainRenderTarget = CreateRenderTarget(GameWidth, GameHeight);
            _tempRenderTarget = CreateRenderTarget(GameWidth, GameHeight);

            if (widthScale < heightScale)
            {
                _display.Width = (int)(GameWidth * widthScale);
                _display.Height = (int)(GameHeight * widthScale);
            }
            else
            {
                _display.Width = (int)(GameWidth * heightScale);
                _display.Height = (int)(GameHeight * heightScale);
            }

            _display.X = width / 2 - _display.Width / 2;
            _display.Y = height / 2 - _display.Height / 2;

            _window.ClientSizeChanged += UpdateDisplay;
        }

        public Matrix GetUIOrigin()
        {
            return Matrix.CreateTranslation(new Vector3((GameWidth / 2) - (VirtualWidth / 2),
                (GameHeight / 2) - (VirtualHeight / 2), 0));
        }
    }
}
