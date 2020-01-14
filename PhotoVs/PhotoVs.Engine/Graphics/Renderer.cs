using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhotoVs.Engine.Graphics
{
    public class Renderer
    {
        private readonly ColorGrading _colorGrading;
        private readonly GraphicsDeviceManager _graphics;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly GameWindow _window;

        public CanvasSize CanvasSize { get; }

        public VirtualRenderTarget2D GameView { get; }
        public VirtualRenderTarget2D UIView { get; }

        public Renderer(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics, GameWindow window,
            ColorGrading colorGrading, CanvasSize canvasSize)
        {
            _graphics = graphics;
            _window = window;
            _graphicsDevice = graphicsDevice;
            _colorGrading = colorGrading;
            GameView = new VirtualRenderTarget2D(graphicsDevice, canvasSize.GetWidth(), canvasSize.GetHeight());
            UIView = new VirtualRenderTarget2D(graphicsDevice, 1280, 720);
            CanvasSize = canvasSize;

            window.ClientSizeChanged += (sender, e) => { UpdateViewports(); };
            UpdateViewports();
        }

        public void SetRenderMode(RenderMode renderMode)
        {
            switch (renderMode)
            {
                case RenderMode.None:
                    _graphicsDevice.SetRenderTarget(null);
                    break;
                case RenderMode.Game:
                    _graphicsDevice.SetRenderTarget(GameView);
                    _graphicsDevice.Clear(Color.Black);
                    break;
                case RenderMode.UI:
                    _graphicsDevice.SetRenderTarget(UIView);
                    _graphicsDevice.Clear(Color.Transparent);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(renderMode), renderMode, null);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var gameView = _colorGrading.Filter(spriteBatch, GameView);

            SetRenderMode(RenderMode.None);
            spriteBatch.Begin(SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            spriteBatch.Draw(gameView, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);
            spriteBatch.Draw(UIView, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);
            spriteBatch.End();
        }

        private void UpdateViewports()
        {
            _graphics.PreferredBackBufferWidth = _window.ClientBounds.Width;
            _graphics.PreferredBackBufferHeight = _window.ClientBounds.Height;
            _graphics.ApplyChanges();

            GameView.UpdateViewport(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
        }
    }
}