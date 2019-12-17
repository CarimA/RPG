using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PhotoVs.Engine.ECS.GameObjects;
using PhotoVs.Logic.Input;
using PhotoVs.Logic.Transforms;

namespace PhotoVs.Logic.PlayerData
{
    public class Player : GameObject
    {
        private readonly Dictionary<string, object> _flags;

        private readonly CPosition _position;
        public readonly CInput Input;
        private readonly float RunSpeed = 295f;
        private readonly float WalkSpeed = 140f;

        private bool _canMove;

        private string _currentLanguage;
        private string _currentMap;
        private string _currentZone;

        public Player()
        {
            Name = "Player";

            _flags = new Dictionary<string, object>();
            _canMove = true;

            _position = new CPosition {Position = new Vector2(0, 100)};
            Components.Add(_position);

            Input = new CInput(new GameInput(PlayerIndex.One)
            {
                KeyMappings =
                {
                    [InputActions.Up] = new List<Keys> {Keys.Up, Keys.W},
                    [InputActions.Down] = new List<Keys> {Keys.Down, Keys.S},
                    [InputActions.Left] = new List<Keys> {Keys.Left, Keys.A},
                    [InputActions.Right] = new List<Keys> {Keys.Right, Keys.D},
                    [InputActions.Action] = new List<Keys> {Keys.Z, Keys.P},
                    [InputActions.Cancel] = new List<Keys> {Keys.X, Keys.O},
                    [InputActions.Run] = new List<Keys> {Keys.X, Keys.O},
                    [InputActions.Fullscreen] = new List<Keys> {Keys.F1},
                    [InputActions.Screenshot] = new List<Keys> {Keys.F12}
                },
                ButtonMappings =
                {
                    [InputActions.Up] = new List<Buttons> {Buttons.DPadUp},
                    [InputActions.Down] = new List<Buttons> {Buttons.DPadDown},
                    [InputActions.Left] = new List<Buttons> {Buttons.DPadLeft},
                    [InputActions.Right] = new List<Buttons> {Buttons.DPadRight},
                    [InputActions.Action] = new List<Buttons> {Buttons.A},
                    [InputActions.Cancel] = new List<Buttons> {Buttons.B},
                    [InputActions.Run] = new List<Buttons> {Buttons.B},
                    [InputActions.Fullscreen] = new List<Buttons>(),
                    [InputActions.Screenshot] = new List<Buttons> {Buttons.Back}
                }
            });

            Components.Add(Input);
            Components.Add(new CVelocity {Velocity = new Vector2(0, 0)});
            Components.Add(CCollisionBound.Circle(16, 8));
            Components.Add(new CSize {Size = new Vector2(32, 32)});
        }

        public void Save()
        {
        }

        public void Load()
        {
        }

        public float CurrentSpeed(bool runToggled)
        {
            return runToggled ? RunSpeed : WalkSpeed;
        }

        public void LockMovement()
        {
            _canMove = false;
        }

        public void UnlockMovement()
        {
            _canMove = true;
        }

        public void SetFlag(string key, object value)
        {
            _flags[key] = value;
        }

        public object GetFlag(string key)
        {
            return _flags[key];
        }

        public bool TryGetFlag(string key, out object value)
        {
            var result = _flags.TryGetValue(key, out var v);
            value = v;
            return result;
        }

        public bool CanMove()
        {
            return _canMove;
        }

        public string GetLanguage()
        {
            return _currentLanguage;
        }

        public void SetLanguage(string language)
        {
            _currentLanguage = language;
        }
    }
}