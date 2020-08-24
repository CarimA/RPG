using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Core;

namespace PhotoVs.Engine.Graphics
{
    public class CanvasSize : IStartup
    {
        private readonly GraphicsDeviceManager _graphics;
        private readonly GameWindow _window;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly VirtualResolution _virtualResolution;

        private Rectangle _virtualDisplay;
        //private Rectangle _trueDisplay;

        public CanvasSize(GameWindow window, GraphicsDevice graphicsDevice, GraphicsDeviceManager graphicsDeviceManager, VirtualResolution virtualResolution)
        {
            _window = window;
            _graphicsDevice = graphicsDevice;
            _graphics = graphicsDeviceManager;
            _virtualResolution = virtualResolution;

            Scale = (float)_graphics.PreferredBackBufferHeight / (float)_virtualResolution.Height;
            VirtualMinHeight = virtualResolution.Height;
            VirtualMinWidth = VirtualMinHeight / 9 * 16;
            // turns out that ultrawide is not actually 21:9, it's about 64:27. Who could've guessed that?
            VirtualMaxWidth = VirtualMinHeight / 9 * 22;
            // extra space for 16:10 screens
            VirtualMaxHeight = VirtualMinWidth / 16 * 10;

            VirtualCurrentWidth = VirtualMinWidth;
            VirtualCurrentHeight = VirtualMinHeight;

            _virtualDisplay = Rectangle.Empty;

            window.ClientSizeChanged += WindowOnClientSizeChanged;
        }


        public float Scale { get; private set; }

        public int VirtualMinWidth { get; }
        public int VirtualMinHeight { get; }
        public int VirtualMaxWidth { get; }
        public int VirtualMaxHeight { get; }
        public int VirtualCurrentWidth { get; private set; }
        public int VirtualCurrentHeight { get; private set; }

        public int TrueMinWidth => (int)(VirtualMinWidth * Scale);
        public int TrueMinHeight => (int)(VirtualMinHeight * Scale);
        public int TrueMaxWidth => (int)(VirtualMaxWidth * Scale);
        public int TrueMaxHeight => (int)(VirtualMaxHeight * Scale);
        public int TrueCurrentWidth => (int)(VirtualCurrentWidth * Scale);
        public int TrueCurrentHeight => (int)(VirtualCurrentHeight * Scale);

        public Rectangle VirtualDisplay => _virtualDisplay;
        //public Rectangle TrueDisplay => _trueDisplay;

        public Action OnResize { get; set; }

        public void Start(IEnumerable<object> bindings)
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

            Scale = (float)_graphics.PreferredBackBufferHeight / (float)_virtualResolution.Height;

            _graphics.PreferredBackBufferWidth = width;
            _graphics.PreferredBackBufferHeight = height;
            _graphics.ApplyChanges();

            var widthScale = width / (double) VirtualMinWidth;
            var heightScale = height / (double) VirtualMinHeight;
            var smallest = Math.Min(widthScale, heightScale);

            if (widthScale < heightScale)
            {
                VirtualCurrentWidth = VirtualMinWidth;
                VirtualCurrentHeight = (int) (height / widthScale);
            }
            else
            {
                VirtualCurrentHeight = VirtualMinHeight;
                VirtualCurrentWidth = (int) (width / heightScale);
            }

            VirtualCurrentWidth = Math.Min(VirtualCurrentWidth, VirtualMaxWidth);
            VirtualCurrentHeight = Math.Min(VirtualCurrentHeight, VirtualMaxHeight);

            _virtualDisplay.Width = (int) (VirtualCurrentWidth * smallest);
            _virtualDisplay.Height = (int) (VirtualCurrentHeight * smallest);
            _virtualDisplay.X = width / 2 - _virtualDisplay.Width / 2;
            _virtualDisplay.Y = height / 2 - _virtualDisplay.Height / 2;

            OnResize?.Invoke();
            _window.ClientSizeChanged += WindowOnClientSizeChanged;
        }
    }
}