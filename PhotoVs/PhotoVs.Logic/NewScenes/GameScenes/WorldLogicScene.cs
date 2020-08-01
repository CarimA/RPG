using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.Core;
using PhotoVs.Engine.ECS;
using PhotoVs.Logic.Mechanics.Camera.Systems;
using PhotoVs.Logic.Mechanics.Input;
using PhotoVs.Logic.Mechanics.Input.Components;
using PhotoVs.Logic.Mechanics.Movement.Systems;
using PhotoVs.Logic.Mechanics.World;

namespace PhotoVs.Logic.NewScenes.GameScenes
{
    public class WorldLogicScene : Scene
    {
        private readonly SCamera _camera;

        public WorldLogicScene(IGameState gameState, IAssetLoader assetLoader, SpriteBatch spriteBatch,
            IOverworld overworld, ISignal signal)
            : base(new SystemList
            {
                new SCollisionDebugRender(assetLoader, spriteBatch, overworld, gameState),
                new SCollisionResolution(overworld, signal, gameState),
                new SProcessInteractionEvents(gameState, overworld, signal)
            })
        {
            _camera = gameState.Camera;
        }

        public override bool IsBlocking => false;

        public override void ProcessInput(GameTime gameTime, CInputState inputState)
        {
            if (inputState.ActionPressed(InputActions.Action)) _camera.SetZoom(_camera.Zoom + 0.25f);

            if (inputState.ActionPressed(InputActions.Cancel)) _camera.SetZoom(_camera.Zoom - 0.25f);

            base.ProcessInput(gameTime, inputState);
        }
    }
}