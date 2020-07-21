using Microsoft.Xna.Framework;
using PhotoVs.Engine;
using PhotoVs.Engine.ECS;
using PhotoVs.Logic.Mechanics.Camera.Systems;
using PhotoVs.Logic.Mechanics.Input;
using PhotoVs.Logic.Mechanics.Input.Components;
using PhotoVs.Logic.Mechanics.Movement.Systems;

namespace PhotoVs.Logic.NewScenes.GameScenes
{
    public class WorldLogicScene : Scene
    {
        public override bool IsBlocking => false;

        private readonly SCamera _camera;

        public WorldLogicScene(Services services)
            : base(new SystemList()
            {
                new SCollisionDebugRender(services),
                new SCollisionResolution(services),
                new SProcessInteractionEvents(services)
            })
        {
            _camera = services.Get<SCamera>();
        }

        public override void ProcessInput(GameTime gameTime, CInputState inputState)
        {
            if (inputState.ActionPressed(InputActions.Action))
            {
                _camera.SetZoom(_camera.Zoom + 0.25f);
            }

            if (inputState.ActionPressed(InputActions.Cancel))
            {
                _camera.SetZoom(_camera.Zoom - 0.25f);
            }

            base.ProcessInput(gameTime, inputState);
        }
    }
}
