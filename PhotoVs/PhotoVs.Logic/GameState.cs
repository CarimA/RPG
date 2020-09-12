using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.Audio;
using PhotoVs.Engine.Core;
using PhotoVs.Engine.ECS;
using PhotoVs.Engine.Events.EventArgs;
using PhotoVs.Engine.Graphics;
using PhotoVs.Logic.Mechanics;
using PhotoVs.Logic.Mechanics.World;
using PhotoVs.Logic.PlayerData;
using PhotoVs.Logic.Scenes;
using PhotoVs.Utils;

namespace PhotoVs.Logic
{
    public class GameState : IStartup, IHasBeforeUpdate
    {
        private readonly ISignal _signal;
        private readonly IOverworld _overworld;
        private readonly IAudio _audio;
        private readonly IRenderer _renderer;
        private readonly SpriteBatch _spriteBatch;
        private readonly GraphicsDevice _graphicsDevice;
        public Stage Stage { get; }

        public GameState(Camera camera, Stage stage, IAssetLoader assetLoader, ISignal signal, IOverworld overworld, IAudio audio,
            IRenderer renderer, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            _signal = signal;
            _overworld = overworld;
            _audio = audio;
            _renderer = renderer;
            _spriteBatch = spriteBatch;
            _graphicsDevice = graphicsDevice;
            Stage = stage;
            Config = Config.Load(assetLoader);
            Player = new Player(Config, assetLoader);

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

        public void Start(IEnumerable<object> bindings)
        {
            _overworld.LoadMaps("maps/");
            _overworld.SetMap("novalondinium");

            Player.PlayerData.Position.Position = new Vector2(8400, 6000);

            Stage.ChangeScene<Test>();
            //_sceneMachine.Push(new WorldScene(_assetLoader, _renderer, _overworld, _spriteBatch, _gameState, _signal,
            //    _graphicsDevice, _canvasSize));
            //_sceneMachine.Push(new TitleScene(_services));
            //_sceneMachine.Push(new WorldLogicScene(_gameState, _assetLoader, _spriteBatch, _overworld, _signal));

            _signal.Notify("GameStart", new GameEventArgs(this));

            //_audio.PlayBgm("key");
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