using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PhotoVs.Engine;
using PhotoVs.Engine.Core;
using PhotoVs.Engine.Graphics;
using PhotoVs.Logic.Mechanics;

namespace PhotoVs.Logic
{
    public class FullscreenHandler : IHasBeforeUpdate, IStartup
    {
        private readonly GraphicsDeviceManager _graphics;
        private readonly IPlatform _platform;
        private readonly GameState _gameState;
        private readonly GameWindow _window;

        private int _lastWindowWidth;
        private int _lastWindowHeight;

        public FullscreenHandler(GraphicsDeviceManager graphics, IPlatform platform, GameState gameState, GameWindow window)
        {
            _graphics = graphics;
            _platform = platform;
            _gameState = gameState;
            _window = window;
        }

        public void Start(IEnumerable<object> bindings)
        {
            if (_platform.OverrideFullscreen || _gameState.Config.Fullscreen)
            {
                EnableFullscreen();
            }
        }

        public int BeforeUpdatePriority { get; set; } = -9999;
        public bool BeforeUpdateEnabled { get; set; } = true;

        public void BeforeUpdate(GameTime gameTime)
        {
            if (_gameState.Player.Input.ActionPressed(InputActions.Fullscreen))
                ToggleFullscreen();
        }

        public void EnableFullscreen()
        {
            _lastWindowWidth = _graphics.PreferredBackBufferWidth;
            _lastWindowHeight = _graphics.PreferredBackBufferHeight;
            _graphics.PreferredBackBufferWidth = _graphics.GraphicsDevice.DisplayMode.Width;
            _graphics.PreferredBackBufferHeight = _graphics.GraphicsDevice.DisplayMode.Height;
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();
        }

        public void DisableFullscreen()
        {
            _graphics.PreferredBackBufferWidth = _lastWindowWidth;
            _graphics.PreferredBackBufferHeight = _lastWindowHeight;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();

            //_window.Position = new Point((_graphics.GraphicsDevice.DisplayMode.Width / 2) - (_lastWindowWidth / 2),
            //    (_graphics.GraphicsDevice.DisplayMode.Height / 2) - (_lastWindowHeight / 2));
        }

        public void ToggleFullscreen()
        {
            if (_graphics.IsFullScreen)
                DisableFullscreen();
            else
                EnableFullscreen();
        }
    }
}