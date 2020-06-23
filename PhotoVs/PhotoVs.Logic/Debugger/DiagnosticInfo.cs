using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PhotoVs.Logic.Debugger
{
    public class DiagnosticInfo
    {
        private readonly PolygonPrimitive _backgroundBar;

        private readonly PolygonPrimitive _backgroundBarOutline;
        private readonly PolygonPrimitive _drawBar;
        private readonly Stopwatch _drawTimer;

        private readonly SpriteFont _font;
        private readonly SpriteBatch _spriteBatch;
        private readonly PolygonPrimitive _textBackground;
        private readonly PolygonPrimitive _updateBar;

        private readonly Stopwatch _updateTimer;
        private int _fps;

        private int _fpsTicks;
        private float _fpsTimer;
        private TimeSpan _lastDraw;

        private TimeSpan _lastUpdate;
        private MainGame _game;

        public DiagnosticInfo(MainGame game, SpriteBatch spriteBatch, IAssetLoader assetLoader)
        {
            _game = game;
            _spriteBatch = spriteBatch;
            _font = assetLoader.Get<SpriteFont>("ui/fonts/bold_12.fnt");

            _updateTimer = new Stopwatch();
            _drawTimer = new Stopwatch();

            _backgroundBarOutline = new PolygonPrimitive(spriteBatch.GraphicsDevice, Color.White);
            _backgroundBar = new PolygonPrimitive(spriteBatch.GraphicsDevice, Color.Black);
            _textBackground = new PolygonPrimitive(spriteBatch.GraphicsDevice, Color.Black * 0.5f);
            _updateBar = new PolygonPrimitive(spriteBatch.GraphicsDevice, Color.Blue);
            _drawBar = new PolygonPrimitive(spriteBatch.GraphicsDevice, Color.Yellow);
        }

        public void BeforeUpdate()
        {
            _updateTimer.Start();
        }

        public void AfterUpdate()
        {
            _lastUpdate = _updateTimer.Elapsed;
            _updateTimer.Reset();
        }

        public void BeforeDraw()
        {
            _drawTimer.Start();
        }

        public void AfterDraw()
        {
            _lastDraw = _drawTimer.Elapsed;
            _drawTimer.Reset();
        }

        public void Draw(GameTime gameTime)
        {
            _fpsTicks++;
            _fpsTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_fpsTimer <= 0)
            {
                _fps = _fpsTicks;
                _fpsTimer = 1.0f;
                _fpsTicks = 0;
            }

            var barWidth = (int)(_spriteBatch.GraphicsDevice.Viewport.Width / 4);
            var x = 20;
            var barHeight = 10;
            var y = _spriteBatch.GraphicsDevice.Viewport.Height - 20 - barHeight;
            _backgroundBarOutline.SetPoints(new List<Vector2>
            {
                new Vector2(x - 1, y - 1),
                new Vector2(x + barWidth + 1, y - 1),
                new Vector2(x + barWidth + 1, y + barHeight + 1),
                new Vector2(x - 1, y + barHeight + 1)
            });
            _backgroundBar.SetPoints(new List<Vector2>
            {
                new Vector2(x, y),
                new Vector2(x + barWidth, y),
                new Vector2(x + barWidth, y + barHeight),
                new Vector2(x, y + barHeight)
            });

            var updateWidth = (int)(barWidth * _game.TargetElapsedTime.TotalSeconds * _lastUpdate.TotalMilliseconds);

            _updateBar.SetPoints(new List<Vector2>
            {
                new Vector2(x, y),
                new Vector2(x + updateWidth, y),
                new Vector2(x + updateWidth, y + barHeight),
                new Vector2(x, y + barHeight)
            });

            var nx = x + updateWidth;
            var drawWidth = (int)(barWidth * _game.TargetElapsedTime.TotalSeconds * _lastDraw.TotalMilliseconds);

            _drawBar.SetPoints(new List<Vector2>
            {
                new Vector2(nx, y),
                new Vector2(nx + drawWidth, y),
                new Vector2(nx + drawWidth, y + barHeight),
                new Vector2(nx, y + barHeight)
            });


            _spriteBatch.Begin();

            _backgroundBarOutline.Draw();
            _backgroundBar.Draw();
            _updateBar.Draw();
            _drawBar.Draw();

            var text =
                $"FPS:          {_fps}\nUpdate Avg.:  {_lastUpdate.TotalMilliseconds.ToString("00.000000")} ms\nDraw Avg.:    {_lastDraw.TotalMilliseconds.ToString("00.000000")} ms";
            var theight = _font.MeasureString(text).Y + 20;
            var ty = y - theight - 20;

            if (_spriteBatch.GraphicsDevice.Viewport.Width > x + 420
                && _spriteBatch.GraphicsDevice.Viewport.Height > ty + theight)
            {
                _textBackground.SetPoints(new List<Vector2>
                {
                    new Vector2(x, ty),
                    new Vector2(x + 420, ty),
                    new Vector2(x + 420, ty + theight),
                    new Vector2(x, ty + theight)
                });

                _textBackground.Draw();
                _spriteBatch.DrawString(_font,
                    text,
                    new Vector2(x + 10, ty + 10),
                    Color.Yellow);
            }

            _spriteBatch.End();
        }
    }
}