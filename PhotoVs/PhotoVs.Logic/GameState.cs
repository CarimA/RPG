using Microsoft.Xna.Framework;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.Core;
using PhotoVs.Engine.ECS;
using PhotoVs.Logic.Mechanics;
using PhotoVs.Logic.PlayerData;
using PhotoVs.Utils;

namespace PhotoVs.Logic
{
    public class GameState : IHasBeforeUpdate
    {
        public Stage Stage { get; }

        public GameState(Camera camera, Stage stage, IAssetLoader assetLoader)
        {
            Stage = stage;
            Config = Config.Load(assetLoader);
            Player = new Player(Config);

            Stage.GameObjects.Add(Player);

            camera.Zoom = 1f;
            camera.Lerp = 0.1f;
            camera.Lead = 0;
            var s = 6f;
            camera.DeadZone = new RectangleF(
                (1f - (1f / s)) / 2f,
                (1f - (1f / s)) / 2f,
                (1f / s), (1f / s));

            //camera.Boundary = new Rectangle(8000, 5800, 800, 800);

            //Camera.SetZoom(canvasSize.DisplayHeight / canvasSize.Height)

            //SceneMachine = new SceneMachine(Player, renderer, CreateGlobalSystems(), CreateGlobalEntities());
        }

        public Config Config { get; }
        public Player Player { get; }
        public GameTime GameTime { get; set; }

        public int BeforeUpdatePriority { get; set; } = int.MinValue;
        public bool BeforeUpdateEnabled { get; set; } = true;

        public void BeforeUpdate(GameTime gameTime)
        {
            GameTime = gameTime;
        }
    }
}