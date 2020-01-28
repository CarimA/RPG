using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Utils;

namespace PhotoVs.Engine.Graphics
{
    public class Renderer
    {
        private readonly ColorGrading _colorGrading;
        private readonly GraphicsDeviceManager _graphics;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly GameWindow _window;

        public CanvasSize CanvasSize { get; }

        public VirtualRenderTarget2D GameView { get; private set; }

        public Size2 RenderSize
        {
            get
            {
                return new Size2(GameView.Width, GameView.Height);
            }
        }

        public Renderer(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics, GameWindow window,
            ColorGrading colorGrading, CanvasSize canvasSize)
        {
            _graphics = graphics;
            _window = window;
            _graphicsDevice = graphicsDevice;
            _colorGrading = colorGrading;
            GameView = new VirtualRenderTarget2D(graphicsDevice, canvasSize.GetWidth(), canvasSize.GetHeight());
            CanvasSize = canvasSize;

            UpdateViewports(null, null);
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
                default:
                    throw new ArgumentOutOfRangeException(nameof(renderMode), renderMode, null);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var width = _graphics.PreferredBackBufferWidth;
            var height = _graphics.PreferredBackBufferHeight;

            var gameView = GameView; //_colorGrading.Filter(spriteBatch, GameView);
            gameView.UpdateViewport(width, height);

            SetRenderMode(RenderMode.None);
            spriteBatch.Begin(SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            gameView.DrawScaled(spriteBatch);
            spriteBatch.End();
        }

        private void UpdateViewports(object sender, EventArgs e)
        {
            // stop stack overflows
            _window.ClientSizeChanged -= UpdateViewports;

            var width = _window.ClientBounds.Width;
            var height = _window.ClientBounds.Height;

            if (width == 0 || height == 0)
            {
                return;
            }

            _graphics.PreferredBackBufferWidth = width;
            _graphics.PreferredBackBufferHeight = height;
            _graphics.ApplyChanges();

            var widthScale = width / (double)CanvasSize.GetWidth();
            var heightScale = height / (double)CanvasSize.GetHeight();

            if (widthScale < heightScale)
            {
                // _viewport.Width = (int)(Width * widthScale);
                // _viewport.Height = (int)(Height * widthScale);
                GameView = new VirtualRenderTarget2D(_graphicsDevice, CanvasSize.GetWidth(), (int)(height / widthScale));
            }
            else
            {
                //_viewport.Width = (int)(Width * heightScale);
                // _viewport.Height = (int)(Height * heightScale);
                GameView = new VirtualRenderTarget2D(_graphicsDevice, (int)(width / heightScale), CanvasSize.GetHeight());
            }

            GameView.UpdateViewport(width, height, false);

            _window.ClientSizeChanged += UpdateViewports;
        }

        public Matrix GetUIOrigin()
        {
            return Matrix.CreateTranslation(new Vector3((GameView.Width / 2) - (CanvasSize.GetWidth() / 2),
                (GameView.Height / 2) - (CanvasSize.GetHeight() / 2), 0));
        }
    }
}