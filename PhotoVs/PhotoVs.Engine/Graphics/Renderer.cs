using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhotoVs.Engine.Graphics
{
    public class Renderer
    {
        //private readonly VirtualRenderTarget2D _uiView;
        public CanvasSize CanvasSize { get; private set; }
        private readonly ColorGrading _colorGrading;

        public VirtualRenderTarget2D GameView { get; private set; }
        public VirtualRenderTarget2D FilterView { get; private set; }
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
            GameView = new VirtualRenderTarget2D(graphicsDevice, canvasSize.GetWidth(), canvasSize.GetHeight()); //);
            //_uiView = new VirtualRenderTarget2D(graphicsDevice, 320 * 2, 180 * 2);
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

            //GameView.DrawScaled(spriteBatch, SamplerState.PointClamp);
            FilterView = _colorGrading.Filter(spriteBatch, GameView);
            FilterView.UpdateViewport(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            FilterView.DrawScaled(spriteBatch, SamplerState.PointClamp);
        }

        private void UpdateViewports()
        {
            _graphics.PreferredBackBufferWidth = _window.ClientBounds.Width;
            _graphics.PreferredBackBufferHeight = _window.ClientBounds.Height;
            _graphics.ApplyChanges();

            GameView.UpdateViewport(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            //_uiView.UpdateViewport(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
        }
    }
}