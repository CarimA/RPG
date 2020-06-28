using Microsoft.Xna.Framework;
using PhotoVs.Engine;
using PhotoVs.Engine.ECS;
using PhotoVs.Logic.Mechanics.Movement.Components;
using PhotoVs.Logic.Mechanics.Input.Components;

namespace PhotoVs.Logic.PlayerData
{
    public class Player : GameObject
    {
        public PlayerData PlayerData { get; }

        private readonly float RunSpeed = 400f;
        private readonly float WalkSpeed = 140f;
        public bool CanMove { get; set; }

        public Player(Services services)
        {
            var config = services.Get<Config>();

            Name = "Player";

            PlayerData = new PlayerData();
            Components.Add(PlayerData.Position);

            CanMove = true;

            Components.Add(CCollisionBound.Circle(16, 8));
            Components.Add(new CSize(new Vector2(32, 32)));
            Components.Add(new CInputState());
            Components.Add(new CKeyboard(config.ControlsKeyboard));
            Components.Add(new CController(PlayerIndex.One, config.ControlsGamepad, ((float)config.Deadzone / 100f)));
            Components.Add(new CInputPriority(InputPriority.GamePad));
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