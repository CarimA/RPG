using Microsoft.Xna.Framework;
using PhotoVs.Engine.Graphics.BitmapFonts;
using PhotoVs.Logic.Input;
using PhotoVs.Models.FSM;

namespace PhotoVs.Logic.Scenes
{
    public class TextInputScene : IUpdateableScene, IDrawableScene
    {
        private const float _sustainTime = 0.8f;
        private const float _repeatTime = 0.25f;

        private readonly char[][] _keyboard =
        {
            "ABCDEFGHIJKLM".ToCharArray(),
            "NOPQRSTUVWXYZ".ToCharArray(),
            "abcdefghijklm".ToCharArray(),
            "nopqrstuvwxyz".ToCharArray(),
            "0123456789-. ".ToCharArray()
        };

        private readonly SceneMachine _scene;

        private int _cursorX;
        private int _cursorY;

        private int _limit;

        private string _question;

        public string Text { get; private set; }
        public bool IsFinished { get; private set; }

        public TextInputScene(SceneMachine scene)
        {
            _scene = scene;
        }

        public void Draw(GameTime gameTime)
        {
            var assetLoader = _scene.Services.AssetLoader;
            var spriteBatch = _scene.Services.SpriteBatch;
            var font = assetLoader.GetAsset<BitmapFont>("fonts/body.fnt");

            const int cellWidth = 16;
            const int cellHeight = 16;

            var offsetX = 320 / 2 - cellWidth * _keyboard[0].Length / 2;
            var offsetY = 180 / 2 - cellHeight * _keyboard.Length / 2 + 20;

            spriteBatch.Begin();

            var questionSize = font.MeasureString(_question).Width;
            var qX = 320 / 2 - questionSize / 2;
            var qY = offsetY - 40;

            spriteBatch.DrawString(font, _question, new Vector2(qX, qY), Color.HotPink);

            var textCellWidth = 14;
            var tX = 320 / 2 - _limit * textCellWidth / 2;
            var tY = qY + 20;

            for (var i = 0; i < _limit; i++)
            {
                var character = (i >= Text.Length
                    ? '_'
                    : Text[i]).ToString();
                var characterSize = font.MeasureString(character).Width;

                spriteBatch.DrawString(font, character, new Vector2(tX + (14 * i - characterSize / 2), tY),
                    Color.White);
            }

            for (var y = 0; y < _keyboard.Length; y++)
            for (var x = 0; x < _keyboard[y].Length; x++)
            {
                var character = _keyboard[y][x].ToString();
                var characterSize = font.MeasureString(character);
                var dX = offsetX + (int) (cellWidth * x + (cellWidth / 2 - characterSize.Width / 2));
                var dY = offsetY + (int) (cellHeight * y + (cellHeight / 2 - characterSize.Height / 2));
                var color = y == _cursorY && x == _cursorX
                    ? Color.Yellow
                    : Color.White;

                spriteBatch.DrawString(font, character, new Vector2(dX, dY), color);
            }

            spriteBatch.End();
        }

        public bool IsBlocking { get; set; } = false;

        public void Enter(params object[] args)
        {
            _question = args[0].ToString();
            _limit = args.Length > 1
                ? int.Parse(args[1].ToString())
                : 15;
            Text = args.Length > 2
                ? args[2].ToString()
                : string.Empty;

            IsFinished = false;
        }

        public void Exit()
        {
        }

        public void Resume()
        {
        }

        public void Suspend()
        {
        }

        public void Update(GameTime gameTime)
        {
            if (IsFinished)
                return;

            var input = _scene.Services.Player.Input;

            if (input.ActionPressed(InputActions.Submit))
                if (Submit())
                    return;

            if (input.ActionPressed(InputActions.Action)) AddCharacter();

            if (input.ActionPressed(InputActions.Cancel)) RemoveCharacter();

            var mX = 0;
            var mY = 0;

            if (input.ActionPressed(InputActions.Up))
                mY--;

            if (input.ActionPressed(InputActions.Down))
                mY++;

            if (input.ActionPressed(InputActions.Left))
                mX--;

            if (input.ActionPressed(InputActions.Right))
                mX++;

            MoveCursor(mX, mY);
        }

        private void AddCharacter()
        {
            if (Text.Length >= _limit)
                return;

            var character = _keyboard[_cursorY][_cursorX];
            Text += character;
        }

        private void RemoveCharacter()
        {
            if (Text.Length > 0)
                Text = Text.Substring(0, Text.Length - 1);
        }

        private void MoveCursor(int x, int y)
        {
            if (x == 0 && y == 0) return;

            _cursorX += x;
            _cursorY += y;

            _cursorY %= _keyboard.Length;
            if (_cursorY < 0)
                _cursorY = _keyboard.Length + _cursorY;

            _cursorX %= _keyboard[_cursorY].Length;
            if (_cursorX < 0)
                _cursorX = _keyboard[_cursorY].Length + _cursorX;
        }

        public bool Submit()
        {
            if (Text.Length > 0)
            {
                IsFinished = true;
                return true;
            }

            return false;
        }
    }
}