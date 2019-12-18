using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace PhotoVs.Engine.Graphics
{
    public class Renderer
    {
        //private readonly VirtualRenderTarget2D _uiView;
        private readonly CanvasSize _canvasSize;
        private readonly ColorGrading _colorGrading;

        private readonly VirtualRenderTarget2D _gameView;
        private readonly GraphicsDeviceManager _graphics;
        private readonly GraphicsDevice _graphicsDevice;

        private readonly GameWindow _window;

        public Renderer(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics, GameWindow window,
            ColorGrading colorGrading, CanvasSize canvasSize)
        {
            _graphics = graphics;
            _window = window;
            _graphicsDevice = graphicsDevice;
            _colorGrading = colorGrading;
            _gameView = new VirtualRenderTarget2D(graphicsDevice, canvasSize.GetWidth(), canvasSize.GetHeight()); //);
            //_uiView = new VirtualRenderTarget2D(graphicsDevice, 320 * 2, 180 * 2);
            _canvasSize = canvasSize;

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
                    _graphicsDevice.SetRenderTarget(_gameView);
                    _graphicsDevice.Clear(Color.Black);
                    break;
                /*case RenderMode.UI:
                    _graphicsDevice.SetRenderTarget(_uiView);
                    _graphicsDevice.Clear(Color.Transparent);
                    break;*/
                default:
                    throw new ArgumentOutOfRangeException(nameof(renderMode), renderMode, null);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            SetRenderMode(RenderMode.None);
            _graphicsDevice.Clear(Color.Black);

            //_gameView.DrawScaled(spriteBatch, SamplerState.PointClamp);
            var filter = _colorGrading.Filter(spriteBatch, _gameView);
            filter.UpdateViewport(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            filter.DrawScaled(spriteBatch, SamplerState.PointClamp);
        }

        private void UpdateViewports()
        {
            _graphics.PreferredBackBufferWidth = _window.ClientBounds.Width;
            _graphics.PreferredBackBufferHeight = _window.ClientBounds.Height;
            _graphics.ApplyChanges();

            _gameView.UpdateViewport(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            //_uiView.UpdateViewport(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
        }

        public CanvasSize GetCanvasSize()
        {
            return _canvasSize;
        }
    }
}