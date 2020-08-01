using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.Graphics;
using PhotoVs.Utils.Logging;

namespace PhotoVs.Logic.Debugger
{
    public class DiagnosticInfo : ILogger
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
        private LogLevel _currentLevel;
        private int _fps;

        private int _fpsTicks;
        private float _fpsTimer;
        private readonly MainGame _game;
        private TimeSpan _lastDraw;

        private TimeSpan _lastUpdate;

        private readonly Queue<string> _logs;
        private readonly int _retain;

        public DiagnosticInfo(MainGame game, SpriteBatch spriteBatch, IAssetLoader assetLoader)
        {
            _logs = new Queue<string>();
            _retain = 15;
            SetLogLevel(LogLevel.Trace);

            //Logger.Write.AddLogger(this);

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

        public void SetLogLevel(LogLevel level)
        {
            _currentLevel = level;
        }

        public void Log(LogLevel level, string message, params object[] args)
        {
            if (level < _currentLevel)
                return;

            _logs.Enqueue(string.Format(
                $"[{DateTime.Now:hh:mm:ss}]\t{Enum.GetName(typeof(LogLevel), level).ToUpperInvariant()}\t\t{message}",
                args));

            if (_logs.Count > _retain)
                _logs.Dequeue();
        }

        public void LogTrace(string message, params object[] args)
        {
            Log(LogLevel.Trace, message, args);
        }

        public void LogInfo(string message, params object[] args)
        {
            Log(LogLevel.Info, message, args);
        }

        public void LogWarn(string message, params object[] args)
        {
            Log(LogLevel.Warn, message, args);
        }

        public void LogError(string message, params object[] args)
        {
            Log(LogLevel.Error, message, args);
        }

        public void LogFatal(string message, params object[] args)
        {
            Log(LogLevel.Fatal, message, args);
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
            _fpsTimer -= (float) gameTime.ElapsedGameTime.TotalSeconds;
            if (_fpsTimer <= 0)
            {
                _fps = _fpsTicks;
                _fpsTimer = 1.0f;
                _fpsTicks = 0;
            }

            var barWidth = _spriteBatch.GraphicsDevice.Viewport.Width / 4;
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

            var updateWidth = (int) (barWidth * _game.TargetElapsedTime.TotalSeconds * _lastUpdate.TotalMilliseconds);

            _updateBar.SetPoints(new List<Vector2>
            {
                new Vector2(x, y),
                new Vector2(x + updateWidth, y),
                new Vector2(x + updateWidth, y + barHeight),
                new Vector2(x, y + barHeight)
            });

            var nx = x + updateWidth;
            var drawWidth = (int) (barWidth * _game.TargetElapsedTime.TotalSeconds * _lastDraw.TotalMilliseconds);

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
            var theight = _font.MeasureString(text).Y;
            var ty = y - theight - 5;
            x = x + 1;

            _spriteBatch.DrawString(_font,
                text,
                new Vector2(x + 1, ty + 1),
                Color.Black);
            _spriteBatch.DrawString(_font,
                text,
                new Vector2(x - 1, ty + 1),
                Color.Black);
            _spriteBatch.DrawString(_font,
                text,
                new Vector2(x + 1, ty - 1),
                Color.Black);
            _spriteBatch.DrawString(_font,
                text,
                new Vector2(x - 1, ty - 1),
                Color.Black);

            _spriteBatch.DrawString(_font,
                text,
                new Vector2(x, ty),
                Color.Yellow);

            var h = _font.MeasureString(" ").Y;
            var i = 0;
            foreach (var log in _logs)
            {
                i++;
                var fy = ty + h * i - _retain * h - h;
                _spriteBatch.DrawString(_font,
                    log,
                    new Vector2(x + 1, fy + 1),
                    Color.Black);
                _spriteBatch.DrawString(_font,
                    log,
                    new Vector2(x - 1, fy + 1),
                    Color.Black);
                _spriteBatch.DrawString(_font,
                    log,
                    new Vector2(x + 1, fy - 1),
                    Color.Black);
                _spriteBatch.DrawString(_font,
                    log,
                    new Vector2(x - 1, fy - 1),
                    Color.Black);

                _spriteBatch.DrawString(_font,
                    log,
                    new Vector2(x, fy),
                    Color.AntiqueWhite);
            }

            _spriteBatch.End();
        }
    }
}