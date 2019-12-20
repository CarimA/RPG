using Microsoft.Xna.Framework;
using PhotoVs.Logic.Input;
using PhotoVs.Models.FSM;

namespace PhotoVs.Logic.Scenes
{
    public class TextInputScene : IUpdateableScene, IDrawableScene
    {
        private SceneMachine _scene;

        private int _cursorX;
        private int _cursorY;

        private const float _sustainTime = 0.8f;
        private const float _repeatTime = 0.25f;

        private string _question;
        private int _limit;
        private string _text;
        private bool _isFinished;

        public bool IsFinished { get => _isFinished; }

        public bool IsBlocking { get; set; } = false;

        private char[][] _keyboard =
        {
            "ABCDEFGHIJKLM".ToCharArray(),
            "NOPQRSTUVWXYZ".ToCharArray(),
            "abcdefghijklm".ToCharArray(),
            "nopqrstuvwxyz".ToCharArray(),
            "0123456789-. ".ToCharArray()
        };

        public TextInputScene(SceneMachine scene)
        {
            _scene = scene;
        }

        public void Enter(params object[] args)
        {
            _question = args[0].ToString();
            _limit = args.Length > 1 
                ? int.Parse(args[1].ToString()) 
                : 15;
            _text = args.Length > 2
                ? args[2].ToString()
                : string.Empty;

            _isFinished = false;
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
            var input = _scene.Services.Player.Input;

            var mX = 0;
            var mY = 0;

            var upHold = input.ActionPressedTime(InputActions.Up);
            var downHold = input.ActionPressedTime(InputActions.Up);
            var leftHold = input.ActionPressedTime(InputActions.Up);
            var rightHold = input.ActionPressedTime(InputActions.Up);

            if (input.ActionPressed(InputActions.Up))
                mY--;

            if (input.ActionPressed(InputActions.Down))
                mY++;

            if (input.ActionPressed(InputActions.Left))
                mX--;

            if (input.ActionPressed(InputActions.Right))
                mX++;

            if (upHold >= _sustainTime && upHold % _repeatTime <= 0.05f)
                mY--;

            if (downHold >= _sustainTime && downHold % _repeatTime <= 0.05f)
                mY++;

            if (leftHold >= _sustainTime && leftHold % _repeatTime <= 0.05f)
                mX--;

            if (rightHold >= _sustainTime && rightHold % _repeatTime <= 0.05f)
                mX++;

            MoveCursor(mX, mY);
        }

        private void MoveCursor(int x, int y)
        {
            if (x == 0 && y == 0)
            {
                return;
            }

            _cursorX += x;
            _cursorY += y;

            _cursorY %= _keyboard.Length;
            _cursorX %= _keyboard[_cursorY].Length;
        }

        public void Draw(GameTime gameTime)
        {

        }
    }
}
