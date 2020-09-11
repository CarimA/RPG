using Microsoft.Xna.Framework;
using PhotoVs.Engine.ECS;
using PhotoVs.Logic.Mechanics.Components;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Logic.Mechanics
{
    public class Movement
    {
        public Movement()
        {
            
        }

        [System(RunOn.Update, typeof(CPosition), typeof(CInputState))]
        public void ProcessMovement(GameTime gameTime, GameObjectList gameObjects)
        {
            foreach (var gameObject in gameObjects)
            {
                var position = gameObject.Components.Get<CPosition>();
                var inputState = gameObject.Components.Get<CInputState>();

                position.Position += inputState.LeftAxis * 58 * gameTime.GetElapsedSeconds();
            }
        }
    }
}
