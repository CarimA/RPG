using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PhotoVs.Engine;
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
        PortugueseBR
    }

    public class Player : GameObject
    {
        private readonly CInput _input;

        private readonly CPosition _position;
        private readonly float RunSpeed = 295f;
        private readonly float WalkSpeed = 140f;
        public Dictionary<string, object> Flags { get; }
        public bool CanMove { get; set; }

        public GameInput Input => _input.Input;

        public Player(Services services)
        {
            var config = services.Get<Config>();

            Name = "Player";

            Flags = new Dictionary<string, object>();
            CanMove = true;

            _position = new CPosition {Position = new Vector2(0, 0)};
            Components.Add(_position);

            _input = new CInput(new GameInput(
                PlayerIndex.One,
                config.ControlsGamepad,
                config.ControlsKeyboard));

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