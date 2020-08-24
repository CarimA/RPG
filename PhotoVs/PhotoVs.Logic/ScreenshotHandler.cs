using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.Core;
using PhotoVs.Engine.Graphics;
using PhotoVs.Logic.Mechanics;
using PhotoVs.Utils.Extensions;
using SpriteFontPlus;

namespace PhotoVs.Logic
{
    public class ScreenshotHandler : IHasBeforeUpdate, IHasAfterDraw
    {
        private readonly GameState _gameState;
        private readonly IRenderer _renderer;
        private readonly CanvasSize _targetCanvasSize;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly SpriteBatch _spriteBatch;
        private bool _shouldScreenshot;
        private readonly DynamicSpriteFont _font;

        public ScreenshotHandler(GameState gameState, IRenderer renderer, CanvasSize targetCanvasSize,
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
                var targetWidth = _targetCanvasSize.TrueMinWidth;
                var targetHeight = _targetCanvasSize.TrueMinHeight;
                TakeScreenshot(targetWidth, targetHeight, filename);
            }
            else
            {
                var targetWidth = _targetCanvasSize.TrueCurrentWidth;
                var targetHeight = _targetCanvasSize.TrueCurrentHeight;
                TakeScreenshot(targetWidth, targetHeight, filename);
            }
        }

        private void TakeScreenshot(int targetWidth, int targetHeight, string filename)
        {
            var text = "Photeus - Monster Collecting RPG in Development - www.play-photeus.com";
            var size = _font.MeasureString(text);

            var backbuffer = _renderer.Backbuffer;

            var scale = 720f / targetHeight;

            var screenshotBuffer = _renderer.CreateRenderTarget((int)(targetWidth * scale), (int)(targetHeight * scale));

            var tX = ((targetWidth * scale) / 2) - (size.X / 2);
            var tY = (targetHeight * scale) - size.Y - 20;

            _graphicsDevice.SetRenderTarget(screenshotBuffer);
            _graphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(backbuffer, new Rectangle(0, 0, (int)(targetWidth * scale), (int)(targetHeight * scale)), Color.White);

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

            screenshotBuffer.Dispose();
        }
    }
}