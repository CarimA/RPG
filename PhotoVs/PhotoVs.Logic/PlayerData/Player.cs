using Microsoft.Xna.Framework;
using PhotoVs.Engine;
using PhotoVs.Engine.ECS.GameObjects;
using PhotoVs.Logic.Mechanics.Input;
using PhotoVs.Logic.Mechanics.Input.Components;
using PhotoVs.Logic.Mechanics.Movement.Components;
using System.Collections.Generic;

namespace PhotoVs.Logic.PlayerData
{
    public class Player : GameObject
    {
        private readonly CInput _input;

        private readonly CPosition _position;
        private readonly float RunSpeed = 150f;
        private readonly float WalkSpeed = 95f;
        public Dictionary<string, object> Flags { get; }
        public bool CanMove { get; set; }

        public GameInput Input => _input.Input;

        public Player(Services services)
        {
            var config = services.Get<Config>();

            Name = "Player";

            Flags = new Dictionary<string, object>();
            CanMove = true;

            _position = new CPosition { Position = new Vector2(0, 0) };
            Components.Add(_position);

            _input = new CInput(new GameInput(
                PlayerIndex.One,
                config.ControlsGamepad,
                config.ControlsKeyboard));

            Components.Add(_input);
            Components.Add(CCollisionBound.Circle(16, 8));
            Components.Add(new CSize { Size = new Vector2(32, 32) });
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