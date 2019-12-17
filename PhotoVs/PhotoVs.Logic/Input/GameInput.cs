using Microsoft.Xna.Framework;
using PhotoVs.Engine.GameInput;

namespace PhotoVs.Logic.Input
{
    public class GameInput : Input<InputActions>
    {
        public GameInput(PlayerIndex playerIndex) : base(playerIndex)
        {

        }

        public Vector2 GetAxis()
        {
            var output = LeftStick;

            if (output != Vector2.Zero)
                return output;

            output.Y -= ActionDown(InputActions.Up) ? 1f : 0;
            output.Y += ActionDown(InputActions.Down) ? 1f : 0;
            output.X -= ActionDown(InputActions.Left) ? 1f : 0;
            output.X += ActionDown(InputActions.Right) ? 1f : 0;

            if (output != Vector2.Zero)
                output.Normalize();

            return output;
        }
    }
}
