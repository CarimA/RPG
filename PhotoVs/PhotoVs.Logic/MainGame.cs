using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonSharp.Interpreter;
using PhotoVs.Engine;
using PhotoVs.Engine.Assets;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.Assets.TypeLoaders;
using PhotoVs.Engine.Audio;
using PhotoVs.Engine.Core;
using PhotoVs.Engine.Events.Coroutines;
using PhotoVs.Engine.Events.EventArgs;
using PhotoVs.Engine.Graphics;
using PhotoVs.Engine.Scripting;
using PhotoVs.Logic.Mechanics.Input;
using PhotoVs.Logic.Mechanics.World;
using PhotoVs.Logic.Modules;
using PhotoVs.Logic.NewScenes;
using PhotoVs.Logic.NewScenes.GameScenes;
using PhotoVs.Logic.Text;

namespace PhotoVs.Logic
{
    public class FullscreenHandler : IHasBeforeUpdate, IStartup
    {
        private readonly GraphicsDeviceManager _graphics;
        private readonly IPlatform _platform;
        private readonly IGameState _gameState;
        private readonly ICanvasSize _canvasSize;

        private int _lastWindowWidth;
        private int _lastWindowHeight;

        public FullscreenHandler(GraphicsDeviceManager graphics, IPlatform platform, IGameState gameState, ICanvasSize canvasSize)
        {
            _graphics = graphics;
            _platform = platform;
            _gameState = gameState;
            _canvasSize = canvasSize;
        }

        public void Start()
        {
            if (_platform.OverrideFullscreen || _gameState.Config.Fullscreen)
            {
                EnableFullscreen();
            }
        }

        public int BeforeUpdatePriority { get; set; } = -9999;
        public bool BeforeUpdateEnabled { get; set; } = true;

        public void BeforeUpdate(GameTime gameTime)
        {
            if (_gameState.Player.Input.ActionPressed(InputActions.Fullscreen))
                ToggleFullscreen();
        }

        public void EnableFullscreen()
        {
            _lastWindowWidth = _graphics.PreferredBackBufferWidth;
            _lastWindowHeight = _graphics.PreferredBackBufferHeight;
            _graphics.PreferredBackBufferWidth = _graphics.GraphicsDevice.DisplayMode.Width;
            _graphics.PreferredBackBufferHeight = _graphics.GraphicsDevice.DisplayMode.Height;
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();
        }

        public void DisableFullscreen()
        {
            _graphics.PreferredBackBufferWidth = _lastWindowWidth;
            _graphics.PreferredBackBufferHeight = _lastWindowHeight;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();
        }

        public void ToggleFullscreen()
        {
            if (_graphics.IsFullScreen)
                DisableFullscreen();
            else
                EnableFullscreen();
        }
    }

    public class StartupSequence : IStartup
    {
        private readonly IGameState _gameState;
        private readonly ISignal _signal;
        private readonly IOverworld _overworld;
        private readonly SceneMachine _sceneMachine;
        private readonly IAudio _audio;
        private readonly IAssetLoader _assetLoader;
        private readonly IRenderer _renderer;
        private readonly SpriteBatch _spriteBatch;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly ICanvasSize _canvasSize;

        public StartupSequence(IGameState gameState, ISignal signal, IOverworld overworld, SceneMachine sceneMachine, IAudio audio, 
            IAssetLoader assetLoader, IRenderer renderer, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, ICanvasSize canvasSize)
        {
            _gameState = gameState;
            _signal = signal;
            _overworld = overworld;
            _sceneMachine = sceneMachine;
            _audio = audio;
            _assetLoader = assetLoader;
            _renderer = renderer;
            _spriteBatch = spriteBatch;
            _graphicsDevice = graphicsDevice;
            _canvasSize = canvasSize;
        }

        public void Start()
        {
            _overworld.LoadMaps("maps/");
            _overworld.SetMap("novalondinium");

            _gameState.Player.PlayerData.Position.Position = new Vector2(8400, 6000);

            _sceneMachine.Push(new WorldScene(_assetLoader, _renderer, _overworld, _spriteBatch, _gameState, _signal,
                _graphicsDevice, _canvasSize));
            //_sceneMachine.Push(new TitleScene(_services));
            _sceneMachine.Push(new WorldLogicScene(_gameState, _assetLoader, _spriteBatch, _overworld, _signal));

            _signal.Notify("GameStart", new GameEventArgs(this));

            _audio.PlayBgm("key");
        }
    }

    public class MainGame : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private readonly Kernel _kernel;
        private readonly IPlatform _platform;
        private readonly Scheduler _scheduler;

        public MainGame(IPlatform platform)
        {
            _platform = platform;

            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            InactiveSleepTime = TimeSpan.Zero;
            IsFixedTimeStep = false;

            _graphics = new GraphicsDeviceManager(this)
            {
                GraphicsProfile = GraphicsProfile.HiDef,
                SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight,
                SynchronizeWithVerticalRetrace = true,
                PreferredBackBufferWidth = 1920,
                PreferredBackBufferHeight = 1080
            };
            _graphics.ApplyChanges();

            _kernel = new Kernel();
            _scheduler = new Scheduler(_kernel);
        }

        protected override void Initialize()
        {
            _kernel
                // monogame specific things
                .Bind<Game>(this)
                .Bind(_graphics)
                .Bind(GraphicsDevice)
                .Bind<SpriteBatch>()
                .Bind(Window)

                // core engine specific things
                .Bind(_platform)
                .Bind<ISignal, Signal>()
                .Bind<FullscreenHandler>()
                .Bind<ICanvasSize, TargetCanvasSize>()
                .Bind<ICoroutineRunner, CoroutineRunner>()
                .Bind<IAssetLoader, AssetLoader>()
                .Bind<IRenderer, Renderer>()
                .Bind<IAudio>(_platform.Audio)
                //.Bind<IAudio, DummyAudio>() // todo: figure out how to substitute from platform
                .Bind<IScriptHost, ScriptHost<Closure>>()
                .Bind<IInterpreter<Closure>, MoonSharpInterpreter>()

                // type loaders for assets
                .Bind<EffectTypeLoader>()
                .Bind<TextTypeLoader>()
                .Bind<Texture2DTypeLoader>()
                .Bind<SpriteFontTypeLoader>()
                .Bind<DynamicSpriteFontTypeLoader>()
                .Bind<MapTypeLoader>()

                // game logic
                .Bind<StartupSequence>()
                .Bind<IGameState, GameState>()
                .Bind<ITextDatabase, TextDatabase>()
                .Bind<SceneMachine>()
                .Bind<IOverworld, Overworld>()

                // modules for scripting
                .Bind<StandardLibraryModule>()
                .Bind<EventConditionsModule>()
                .Bind<EventTriggersModule>()
                .Bind<SceneMachineModule>()
                .Bind<TimingModule>()
                .Bind<DialogueModule>()
                .Bind<PlayerModule>()
                .Bind<GameObjectModule>()
                .Bind<TextModule>()

                // data that will get used to initialise state
                .Bind(new VirtualGameSize(640, 360))
                .Bind(new ScriptData(new List<(DataLocation, string)>
                {
                    (DataLocation.Content, "logic/"),
                    (DataLocation.Storage, "mods/")
                }));

            // todo: substitute anything provided from platform

            _kernel.Construct();
            _scheduler.Start();

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            _scheduler.BeforeUpdate(gameTime);
            _scheduler.Update(gameTime);
            _scheduler.AfterUpdate(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _scheduler.BeforeDraw(gameTime);
            _scheduler.Draw(gameTime);
            _scheduler.AfterDraw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            _kernel.Dispose();
            base.OnExiting(sender, args);
        }
    }
}