using System;
using Microsoft.Xna.Framework;
using PhotoVs.Engine.Core;

namespace PhotoVs.Engine.Graphics
{
    public class TargetCanvasSize : ICanvasSize, IStartup
    {
        private readonly GraphicsDeviceManager _graphics;
        private readonly GameWindow _window;
        private readonly VirtualGameSize _virtualGameSize;

        private Rectangle _display;

        public TargetCanvasSize(GameWindow window, GraphicsDeviceManager graphicsDeviceManager, VirtualGameSize virtualGameSize)
        {
            _window = window;
            _graphics = graphicsDeviceManager;

            Height = virtualGameSize.BackbufferHeight;
            Width = Height / 9 * 16;
            // turns out that ultrawide is not actually 21:9, it's about 64:27. Who could've guessed that?
            MaxWidth = Height / 27 * 69;
            // extra space for 16:10 screens
            MaxHeight = Width / 16 * 10;

            _display = new Rectangle();
            DisplayWidth = Width;
            DisplayHeight = Height;

            window.ClientSizeChanged += WindowOnClientSizeChanged;
        }

        public int Width { get; }
        public int Height { get; }
        public int MaxWidth { get; }
        public int MaxHeight { get; }
        public Rectangle DisplayRectangle => _display;
        public int DisplayWidth { get; private set; }
        public int DisplayHeight { get; private set; }

        public Action OnResize { get; set; }

        public void Start()
        {
            WindowOnClientSizeChanged(null, null);
        }

        private void WindowOnClientSizeChanged(object sender, EventArgs e)
        {
            _window.ClientSizeChanged -= WindowOnClientSizeChanged;

            var width = _window.ClientBounds.Width;
            var height = _window.ClientBounds.Height;

            if (width == 0 || height == 0)
            {
                _window.ClientSizeChanged += WindowOnClientSizeChanged;
                return;
            }

            _graphics.PreferredBackBufferWidth = width;
            _graphics.PreferredBackBufferHeight = height;
            _graphics.ApplyChanges();

            var widthScale = width / (double) Width;
            var heightScale = height / (double) Height;
            var smallest = Math.Min(widthScale, heightScale);

            if (widthScale < heightScale)
            {
                DisplayWidth = Width;
                DisplayHeight = (int) (height / widthScale);
            }
            else
            {
                DisplayHeight = Height;
                DisplayWidth = (int) (width / heightScale);
            }

            DisplayWidth = Math.Min(DisplayWidth, MaxWidth);
            DisplayHeight = Math.Min(DisplayHeight, MaxHeight);

            _display.Width = (int) (DisplayWidth * smallest);
            _display.Height = (int) (DisplayHeight * smallest);
            _display.X = width / 2 - _display.Width / 2;
            _display.Y = height / 2 - _display.Height / 2;

            OnResize?.Invoke();
            _window.ClientSizeChanged += WindowOnClientSizeChanged;
        }
    }
}