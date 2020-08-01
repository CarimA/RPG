using Microsoft.Xna.Framework;
using PhotoVs.Engine;
using PhotoVs.Engine.Core;
using PhotoVs.Engine.Graphics;
using PhotoVs.Logic.Mechanics.Input;

namespace PhotoVs.Logic
{
    public class FullscreenHandler : IHasBeforeUpdate, IStartup
    {
        private readonly GraphicsDeviceManager _graphics;
        private readonly IPlatform _platform;
        private readonly IGameState _gameState;
        private readonly ICanvasSize _canvasSize;

        private int _lastWindowWidth;
        private int _lastWindowHeight;

        public FullscreenHandler(GraphicsDeviceManager graphics, IPlatform platform, IGameState gameState, ICanvasSize canvasSize)
        {
            _graphics = graphics;
            _platform = platform;
            _gameState = gameState;
            _canvasSize = canvasSize;
        }

        public void Start()
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