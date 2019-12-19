using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Graphics;
using PhotoVs.Engine.Graphics.BitmapFonts;
using PhotoVs.Models.Assets;

namespace PhotoVs.Logic.Debug
{
    public class DiagnosticInfo
    {
        private SpriteBatch _spriteBatch;

        private BitmapFont _font;

        private Stopwatch _updateTimer;
        private Stopwatch _drawTimer;

        private int _fpsTicks;
        private float _fpsTimer;
        private int _fps;

        private TimeSpan _lastUpdate;
        private TimeSpan _lastDraw;

        private PolygonPrimitive _backgroundBarOutline;
        private PolygonPrimitive _backgroundBar;
        private PolygonPrimitive _textBackground;
        private PolygonPrimitive _updateBar;
        private PolygonPrimitive _drawBar;

        public DiagnosticInfo(SpriteBatch spriteBatch, IAssetLoader assetLoader)
        {
            _spriteBatch = spriteBatch;
            _font = assetLoader.GetAsset<BitmapFont>("fonts/mono.fnt");

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

            int barWidth = (int)(_spriteBatch.GraphicsDevice.Viewport.Width / 1.5);
            int x = (_spriteBatch.GraphicsDevice.Viewport.Width / 2) - (barWidth / 2);
            int barHeight = 10;
            int y = _spriteBatch.GraphicsDevice.Viewport.Height - 40 - barHeight;
            _backgroundBarOutline.SetPoints(new List<Vector2>()
            {
                new Vector2(x - 1, y - 1),
                new Vector2(x + barWidth + 1, y - 1),
                new Vector2(x + barWidth + 1, y + barHeight + 1),
                new Vector2(x - 1, y + barHeight + 1)
            });
            _backgroundBar.SetPoints(new List<Vector2>()
            {
                new Vector2(x, y),
                new Vector2(x + barWidth, y),
                new Vector2(x + barWidth, y + barHeight),
                new Vector2(x, y + barHeight)
            });

            var updateWidth = (int)((barWidth / 60f) * (_lastUpdate.TotalMilliseconds));

            _updateBar.SetPoints(new List<Vector2>()
            {
                new Vector2(x, y),
                new Vector2(x + updateWidth, y),
                new Vector2(x + updateWidth, y + barHeight),
                new Vector2(x, y + barHeight)
            });

            var nx = x + updateWidth;
            var drawWidth = (int)((barWidth / 60f) * (_lastDraw.TotalMilliseconds));

            _drawBar.SetPoints(new List<Vector2>()
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

            var text = $"FPS:         {_fps}\nUpdate Avg.: {_lastUpdate.TotalMilliseconds}ms\nDraw Avg.:   {_lastDraw.TotalMilliseconds}ms";
            var theight = _font.MeasureString(text).Height + 20;
            var ty = y - theight - 20;

            if (_spriteBatch.GraphicsDevice.Viewport.Width > x + 420
                && _spriteBatch.GraphicsDevice.Viewport.Height > ty + theight)
            {
                _textBackground.SetPoints(new List<Vector2>()
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
