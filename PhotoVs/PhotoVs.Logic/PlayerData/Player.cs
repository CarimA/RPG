using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PhotoVs.Engine.ECS.GameObjects;
using PhotoVs.Logic.Input;
using PhotoVs.Logic.Transforms;

namespace PhotoVs.Logic.PlayerData
{
    public enum Languages
    {
        EnglishUK,
        French,
        Spanish,
        Italian,
        German,
        ChineseTD,
        ChineseSD,
        Korean,
        Japanese,
        Russian,
        PortugeuseBR
    }

    public class Player : GameObject
    {
        private readonly CInput _input;

        private readonly CPosition _position;
        private readonly float RunSpeed = 295f;
        private readonly float WalkSpeed = 140f;
        public Dictionary<string, object> Flags { get; }
        public bool CanMove { get; set; }
        public Languages Language { get; set; }

        public GameInput Input => _input.Input;

        public Player()
        {
            Name = "Player";

            Flags = new Dictionary<string, object>();
            CanMove = true;
            Language = Languages.EnglishUK;

            _position = new CPosition {Position = new Vector2(0, 0)};
            Components.Add(_position);

            _input = new CInput(new GameInput(PlayerIndex.One,
                new Dictionary<InputActions, List<Buttons>>
                {
                    [InputActions.Up] = new List<Buttons> {Buttons.DPadUp},
                    [InputActions.Down] = new List<Buttons> {Buttons.DPadDown},
                    [InputActions.Left] = new List<Buttons> {Buttons.DPadLeft},
                    [InputActions.Right] = new List<Buttons> {Buttons.DPadRight},
                    [InputActions.Action] = new List<Buttons> {Buttons.A},
                    [InputActions.Submit] = new List<Buttons> {Buttons.Start},
                    [InputActions.Cancel] = new List<Buttons> {Buttons.B},
                    [InputActions.Run] = new List<Buttons> {Buttons.B},
                    [InputActions.Fullscreen] = new List<Buttons> {Buttons.LeftShoulder},
                    [InputActions.Screenshot] = new List<Buttons> {Buttons.Back}
                },
                new Dictionary<InputActions, List<Keys>>
                {
                    [InputActions.Up] = new List<Keys> {Keys.Up, Keys.W},
                    [InputActions.Down] = new List<Keys> {Keys.Down, Keys.S},
                    [InputActions.Left] = new List<Keys> {Keys.Left, Keys.A},
                    [InputActions.Right] = new List<Keys> {Keys.Right, Keys.D},
                    [InputActions.Action] = new List<Keys> {Keys.Z, Keys.P},
                    [InputActions.Submit] = new List<Keys> {Keys.Enter},
                    [InputActions.Cancel] = new List<Keys> {Keys.X, Keys.O},
                    [InputActions.Run] = new List<Keys> {Keys.X, Keys.O},
                    [InputActions.Fullscreen] = new List<Keys> {Keys.F1},
                    [InputActions.Screenshot] = new List<Keys> {Keys.F12}
                }));

            Components.Add(_input);
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
            CanMove = false;
        }

        public void UnlockMovement()
        {
            CanMove = true;
        }
    }
}