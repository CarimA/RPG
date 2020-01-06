using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Models.Assets;

namespace PhotoVs.Engine.Graphics
{
    public class Renderer
    {
        private readonly ColorGrading _colorGrading;
        private readonly GraphicsDeviceManager _graphics;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly IAssetLoader _assetLoader;
        private readonly GameWindow _window;

        //private readonly VirtualRenderTarget2D _uiView;
        public CanvasSize CanvasSize { get; }

        public VirtualRenderTarget2D GameView { get; }
        public VirtualRenderTarget2D FilterView { get; private set; }

        private RenderTarget2D _final;
        private Effect _scanlines;
        private Effect _crt;

        public Renderer(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics, GameWindow window,
            ColorGrading colorGrading, CanvasSize canvasSize, IAssetLoader assetLoader)
        {
            _graphics = graphics;
            _window = window;
            _graphicsDevice = graphicsDevice;
            _colorGrading = colorGrading;
            _assetLoader = assetLoader;
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
            _crt = _assetLoader.GetAsset<Effect>("shaders/crt.dx11");

            SetRenderMode(RenderMode.None);
            _graphicsDevice.Clear(Color.Black);

            //GameView.DrawScaled(spriteBatch, SamplerState.PointClamp);
            FilterView = _colorGrading.Filter(spriteBatch, GameView);
            FilterView.UpdateViewport(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

            _graphicsDevice.SetRenderTarget(_final);
            spriteBatch.Begin(SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            _crt.CurrentTechnique.Passes[1].Apply();
            FilterView.DrawScaled(spriteBatch);
            spriteBatch.End();

            _graphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin(SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            _crt.CurrentTechnique.Passes[0].Apply();
            spriteBatch.Draw(_final, Vector2.Zero, Color.White);
            spriteBatch.End();

        }

        private void UpdateViewports()
        {
            _graphics.PreferredBackBufferWidth = _window.ClientBounds.Width;
            _graphics.PreferredBackBufferHeight = _window.ClientBounds.Height;
            _graphics.ApplyChanges();

            GameView.UpdateViewport(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            _final = new RenderTarget2D(_graphicsDevice, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            //_uiView.UpdateViewport(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
        }
    }
}