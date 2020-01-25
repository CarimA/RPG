using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Logic.Input;
using PhotoVs.Logic.PlayerData;
using PhotoVs.Models.Assets;
using PhotoVs.Models.FSM;

namespace PhotoVs.Logic.Scenes
{
    public class TextInputScene : IUpdateableScene, IDrawableScene
    {
        private const int _keysPerRow = 12;
        private readonly IAssetLoader _assetLoader;

        // | is inaccessible
        // ^ is shift
        // & is next keyboard
        // £ is delete
        // $ is submit
        private readonly List<string> _keyboards = new List<string>
        {
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ          ^^&&££££$$$$",
            "abcdefghijklmnopqrstuvwxyz          ^^&&££££$$$$",
            "0123456789  .!?-_+=                 ^^&&££££$$$$"
        };

        private readonly Player _player;
        private readonly SpriteBatch _spriteBatch;

        private int _currentKeyboard;
        private int _cursorX;
        private int _cursorY;

        private int _limit;

        private string _question;

        private bool _shiftMode;

        public string Text { get; private set; }
        public bool IsFinished { get; private set; }

        public TextInputScene(SceneMachine scene)
        {
            _player = scene.Services.Get<Player>();
            _assetLoader = scene.Services.Get<IAssetLoader>();
            _spriteBatch = scene.Services.Get<SpriteBatch>();
        }

        public void Draw(GameTime gameTime)
        {
        }

        public void DrawUI(GameTime gameTime)
        {
            var font = _assetLoader.GetAsset<SpriteFont>("ui/fonts/plain_12.fnt");

            const int cellWidth = 18;
            const int cellHeight = 28;

            var offsetX = 320 / 2 - cellWidth * KeyboardCellWidth() / 2;
            var offsetY = 180 / 2 - cellHeight * KeyboardCellHeight() / 2 + 20;

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            var questionSize = font.MeasureString(_question).X;
            var qX = (int) (320 / 2 - questionSize / 2);
            var qY = offsetY - 40;

            _spriteBatch.DrawString(font, _question, new Vector2(qX, qY), Color.HotPink);

            var textCellWidth = 14;
            var tX = 320 / 2 - _limit * textCellWidth / 2;
            var tY = qY + 20;

            for (var i = 0; i < _limit; i++)
            {
                var character = (i >= Text.Length
                    ? '_'
                    : Text[i]).ToString();
                var characterSize = font.MeasureString(character).X;

                _spriteBatch.DrawString(font, character, new Vector2((int) (tX + (14 * i - characterSize / 2)), tY),
                    Color.White);
            }

            for (var y = 0; y < KeyboardCellHeight(); y++)
            for (var x = 0; x < KeyboardCellWidth(); x++)
            {
                // ^ is shift
                // & is next keyboard
                // £ is delete
                // $ is submit
                var character = GetKey(x, y);

                switch (character)
                {
                    case "|":
                        continue;

                    case "^":
                        character = "^";
                        break;

                    case "&":
                        character = "->";
                        break;

                    case "£":
                        character = "<-";
                        break;

                    case "$":
                        character = "SUBMIT";
                        break;
                }

                var characterSize = font.MeasureString(character);
                var dX = offsetX + (int) (cellWidth * x + (cellWidth / 2 - characterSize.X / 2));
                var dY = offsetY + (int) (cellHeight * y + (cellHeight / 2 - characterSize.Y / 2));
                var color = y == _cursorY && x == _cursorX
                    ? Color.Yellow
                    : Color.White;

                _spriteBatch.DrawString(font, character, new Vector2(dX, dY), color);
            }

            _spriteBatch.End();
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

            _shiftMode = true;
            _currentKeyboard = 0;

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

            var input = _player.Input;

            if (input.ActionPressed(InputActions.Submit))
                if (Submit())
                    return;

            if (input.ActionPressed(InputActions.Action))
                AddCharacter();

            if (input.ActionPressed(InputActions.Cancel))
                RemoveCharacter();

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

        private int KeyboardCellWidth()
        {
            return _keysPerRow;
        }

        private int KeyboardCellHeight()
        {
            return _keyboards[_currentKeyboard].Length / _keysPerRow;
        }

        private string GetKey(int x, int y)
        {
            var index = _keysPerRow * y + x % _keysPerRow;
            return _keyboards[_currentKeyboard][index].ToString();
        }

        private void AddCharacter()
        {
            if (Text.Length >= _limit)
                return;

            var character = GetKey(_cursorX, _cursorY);

            // ^ is shift
            // & is next keyboard
            // £ is delete
            // $ is submit

            switch (character)
            {
                case "^":
                    _shiftMode = !_shiftMode;
                    if (_shiftMode)
                        _currentKeyboard = 0;
                    else
                        _currentKeyboard = 1;
                    break;

                case "&":
                    _currentKeyboard++;
                    if (_currentKeyboard >= _keyboards.Count)
                        _currentKeyboard = 0;
                    break;

                case "£":
                    RemoveCharacter();
                    break;

                case "$":
                    Submit();
                    break;

                default:
                    Text += character;

                    if (_shiftMode)
                    {
                        _shiftMode = false;
                        _currentKeyboard = 1;
                    }

                    break;
            }
        }

        private void RemoveCharacter()
        {
            if (Text.Length > 0)
                Text = Text.Substring(0, Text.Length - 1);
        }

        private void MoveCursor(int x, int y)
        {
            // | is inaccessible

            if (x == 0 && y == 0)
                return;
            var lastKey = GetKey(_cursorX, _cursorY);

            _cursorX += x;
            _cursorY += y;

            _cursorY %= KeyboardCellHeight();
            if (_cursorY < 0)
                _cursorY = KeyboardCellHeight() + _cursorY;

            _cursorX %= KeyboardCellWidth();
            if (_cursorX < 0)
                _cursorX = KeyboardCellWidth() + _cursorX;

            var curKey = GetKey(_cursorX, _cursorY);
            if (curKey != " " && curKey == lastKey)
                MoveCursor(x, y);
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