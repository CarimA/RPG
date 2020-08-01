using Microsoft.Xna.Framework;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.Core;
using PhotoVs.Engine.ECS;
using PhotoVs.Engine.Graphics;
using PhotoVs.Logic.Mechanics.Camera.Systems;
using PhotoVs.Logic.Mechanics.Input.Systems;
using PhotoVs.Logic.PlayerData;

namespace PhotoVs.Logic
{
    public class GameState : IGameState, IHasBeforeUpdate
    {
        public GameState(ICanvasSize canvasSize, IAssetLoader assetLoader, ISignal signal, VirtualGameSize virtualGameSize, ICanvasSize targetCanvasSize)
        {
            Config = Config.Load(assetLoader);
            Player = new Player(Config);
            GameObjects = new GameObjectList
            {
                Player
            };

            Camera = new SCamera(canvasSize);
            Camera.Follow(Player);
            Camera.SetZoom((float)targetCanvasSize.Width / virtualGameSize.Width);
            //Camera.SetZoom(canvasSize.DisplayHeight / canvasSize.Height);

            Systems = new SystemList
            {
                Camera,
                new SProcessInputState(),
                new SProcessController(),
                new SProcessKeyboard(),
                new SRaiseInputEvents(signal)
            };

            //SceneMachine = new SceneMachine(Player, renderer, CreateGlobalSystems(), CreateGlobalEntities());
        }

        public Config Config { get; }
        public Player Player { get; }
        public SCamera Camera { get; }

        //public SceneMachine SceneMachine { get; }
        public GameObjectList GameObjects { get; }
        public SystemList Systems { get; }
        public GameTime GameTime { get; set; }

        public int BeforeUpdatePriority { get; set; } = int.MinValue;
        public bool BeforeUpdateEnabled { get; set; } = true;

        public void BeforeUpdate(GameTime gameTime)
        {
            GameTime = gameTime;
        }
    }
}