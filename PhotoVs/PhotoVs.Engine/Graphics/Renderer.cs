using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.Graphics.Filters;

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
        public int GameWidth { get; private set; }
        public int GameHeight { get; private set; }

        private Rectangle _display;

        private RenderTarget2D _mainRenderTarget;
        private RenderTarget2D _tempRenderTarget;

        //private ColorGradingFilter _globalFilter;

        public Renderer(Services services, int virtualWidth, int virtualHeight)
        {
            GraphicsDevice = services.Get<GraphicsDevice>();
            SpriteBatch = services.Get<SpriteBatch>();
            _graphics = services.Get<GraphicsDeviceManager>();
            _window = services.Get<GameWindow>();
            VirtualWidth = virtualWidth;
            VirtualHeight = virtualHeight;

            _display = new Rectangle();
            UpdateDisplay(null, null);

            //var platform = services.Get<IPlatform>();
            //var assetLoader = services.Get<IAssetLoader>();
            //_globalFilter = new ColorGradingFilter(this, 
            //    assetLoader.Get<Effect>(platform.PaletteShader));
            //_globalFilter.LookupTable = assetLoader.Get<Texture2D>("ui/luts/aap128.png");
        }

        public void Draw()
        {
            var width = _graphics.PreferredBackBufferWidth;
            var height = _graphics.PreferredBackBufferHeight;

            //var gameview = _globalFilter.Filter(SpriteBatch, _mainRenderTarget);

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);
            SpriteBatch.Begin(SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            SpriteBatch.Draw(_mainRenderTarget, _display, Color.White);
            SpriteBatch.End();
        }

        public void BeforeDraw()
        {
            GraphicsDevice.SetRenderTarget(_mainRenderTarget);
            GraphicsDevice.Clear(Color.Black);
        }

        public void RequestSubRenderer(RenderTarget2D renderTarget)
        {
            // copy the existing buffer to a temporary buffer so
            // that it isn't wiped
            GraphicsDevice.SetRenderTarget(_tempRenderTarget);
            GraphicsDevice.Clear(Color.Black);
            SpriteBatch.Begin();
            SpriteBatch.Draw(_mainRenderTarget, Vector2.Zero, Color.White);
            SpriteBatch.End();

            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(Color.Black);
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

            var widthScale = width / (double) VirtualWidth;
            var heightScale = height / (double) VirtualHeight;

            /*_mainRenderTarget = widthScale < heightScale 
                ? CreateRenderTarget(VirtualWidth, (int)(height / widthScale)) 
                : CreateRenderTarget((int)(width / heightScale), VirtualHeight);*/

            if (widthScale < heightScale)
            {
                _mainRenderTarget = CreateRenderTarget(VirtualWidth, (int) (height / widthScale));
                _tempRenderTarget = CreateRenderTarget(VirtualWidth, (int) (height / widthScale));
            }
            else
            {
                _mainRenderTarget = CreateRenderTarget((int) (width / heightScale), VirtualHeight);
                _tempRenderTarget = CreateRenderTarget((int) (width / heightScale), VirtualHeight);
            }

            GameWidth = _mainRenderTarget.Width;
            GameHeight = _mainRenderTarget.Height;

            _display.Width = width;
            _display.Height = height;
            _display.X = 0;
            _display.Y = 0;

            _window.ClientSizeChanged += UpdateDisplay;
        }

        public Matrix GetUIOrigin()
        {
            return Matrix.CreateTranslation(new Vector3((GameWidth / 2) - (VirtualWidth / 2),
                (GameHeight / 2) - (VirtualHeight / 2), 0));
        }
    }
}
