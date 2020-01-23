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
        public VirtualRenderTarget2D UIView { get; }

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
            UIView = new VirtualRenderTarget2D(graphicsDevice, canvasSize.GetWidth(), canvasSize.GetHeight());
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
            var width = _graphics.PreferredBackBufferWidth;
            var height = _graphics.PreferredBackBufferHeight;
            var gameView = _colorGrading.Filter(spriteBatch, GameView);
            gameView.UpdateViewport(width, height);

            SetRenderMode(RenderMode.None);
            spriteBatch.Begin(SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            gameView.DrawScaled(spriteBatch);
            UIView.DrawScaled(spriteBatch);
            spriteBatch.End();
        }

        private void UpdateViewports()
        {
            var width = _window.ClientBounds.Width;
            var height = _window.ClientBounds.Height;

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
            UIView.UpdateViewport(width, height);
        }
    }
}