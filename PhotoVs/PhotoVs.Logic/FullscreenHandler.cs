using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.Core;
using PhotoVs.Engine.Graphics;
using PhotoVs.Logic.Mechanics.Input;
using PhotoVs.Utils.Extensions;
using SpriteFontPlus;

namespace PhotoVs.Logic
{
    public class ScreenshotHandler : IHasBeforeUpdate, IHasAfterDraw
    {
        private readonly IGameState _gameState;
        private readonly IRenderer _renderer;
        private readonly ICanvasSize _targetCanvasSize;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly SpriteBatch _spriteBatch;
        private bool _shouldScreenshot;
        private DynamicSpriteFont _font;

        public ScreenshotHandler(IGameState gameState, IRenderer renderer, ICanvasSize targetCanvasSize,
            GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, IAssetLoader assetLoader)
        {
            _gameState = gameState;
            _renderer = renderer;
            _targetCanvasSize = targetCanvasSize;
            _graphicsDevice = graphicsDevice;
            _spriteBatch = spriteBatch;

            _font = assetLoader.Get<DynamicSpriteFont>("ui/fonts/ubuntu.ttf");
        }

        public int BeforeUpdatePriority { get; set; } = 0;
        public bool BeforeUpdateEnabled { get; set; } = true;

        public void BeforeUpdate(GameTime gameTime)
        {
            if (_gameState.Player.Input.ActionPressed(InputActions.Screenshot))
                _shouldScreenshot = true;
        }

        public int AfterDrawPriority { get; set; } = int.MaxValue;
        public bool AfterDrawEnabled { get; set; } = true;

        public void AfterDraw(GameTime gameTime)
        {
            if (_shouldScreenshot)
            {
                TakeScreenshot(false, Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    $"PhotoVs/Screenshots/{DateTime.Now.ToString("yyyyMMdd-HHmmss")}-{Guid.NewGuid().ToString()}.png"));
                _shouldScreenshot = false;
            }
        }

        public void TakeScreenshot(bool useVirtualResolution, string filename)
        {
            if (useVirtualResolution)
            {
                var targetWidth = _targetCanvasSize.Width;
                var targetHeight = _targetCanvasSize.Height;
                TakeScreenshot(targetWidth, targetHeight, filename);
            }
            else
            {
                var targetWidth = _targetCanvasSize.DisplayWidth;
                var targetHeight = _targetCanvasSize.DisplayHeight;
                TakeScreenshot(targetWidth, targetHeight, filename);
            }
        }

        private void TakeScreenshot(int targetWidth, int targetHeight, string filename)
        {
            var text = "Photeus - Monster Collecting RPG in Development - www.play-photeus.com";
            var size = _font.MeasureString(text);

            var backbuffer = _renderer.Backbuffer;
            var x = (targetWidth / 2) - (backbuffer.Width / 2);
            var y = (targetHeight / 2) - (backbuffer.Height / 2);

            var tX = (targetWidth / 2) - (size.X / 2);
            var tY = targetHeight - size.Y - 20;

            var screenshotBuffer = _renderer.CreateRenderTarget(targetWidth, targetHeight);

            _graphicsDevice.SetRenderTarget(screenshotBuffer);
            _graphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            _spriteBatch.Draw(backbuffer, new Vector2(x, y), Color.White);

            var borderSize = 2;
            _spriteBatch.DrawString(_font, text, new Vector2(tX - borderSize, tY - borderSize), Color.Black);
            _spriteBatch.DrawString(_font, text, new Vector2(tX - borderSize, tY + borderSize), Color.Black);
            _spriteBatch.DrawString(_font, text, new Vector2(tX + borderSize, tY - borderSize), Color.Black);
            _spriteBatch.DrawString(_font, text, new Vector2(tX + borderSize, tY + borderSize), Color.Black);
            _spriteBatch.DrawString(_font, text, new Vector2(tX - borderSize, tY), Color.Black);
            _spriteBatch.DrawString(_font, text, new Vector2(tX + borderSize, tY), Color.Black);
            _spriteBatch.DrawString(_font, text, new Vector2(tX, tY - borderSize), Color.Black);
            _spriteBatch.DrawString(_font, text, new Vector2(tX, tY + borderSize), Color.Black);

            _spriteBatch.DrawString(_font, text, new Vector2(tX, tY), Color.Cyan);
            _spriteBatch.End();

            _graphicsDevice.SetRenderTarget(null);

            screenshotBuffer.SaveAsPng(filename);

            screenshotBuffer.UploadToDiscord(_gameState.Config.ImgurClientId, _gameState.Config.DiscordWebhookUrl);
        }
    }

    public class FullscreenHandler : IHasBeforeUpdate, IStartup
    {
        private readonly GraphicsDeviceManager _graphics;
        private readonly IPlatform _platform;
        private readonly IGameState _gameState;
        private readonly ICanvasSize _canvasSize;
        private readonly GameWindow _window;

        private int _lastWindowWidth;
        private int _lastWindowHeight;

        public FullscreenHandler(GraphicsDeviceManager graphics, IPlatform platform, IGameState gameState, ICanvasSize canvasSize, GameWindow window)
        {
            _graphics = graphics;
            _platform = platform;
            _gameState = gameState;
            _canvasSize = canvasSize;
            _window = window;
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