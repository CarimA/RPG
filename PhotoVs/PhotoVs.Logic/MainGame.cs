using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PhotoVs.Engine;
using PhotoVs.Engine.Assets;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.Assets.TypeLoaders;
using PhotoVs.Engine.Audio;
using PhotoVs.Engine.ECS;
using PhotoVs.Engine.ECS.Systems;
using PhotoVs.Engine.Events;
using PhotoVs.Engine.Events.Coroutines;
using PhotoVs.Engine.Events.EventArgs;
using PhotoVs.Engine.Graphics;
using PhotoVs.Engine.Scripting;
using PhotoVs.Engine.TiledMaps;
using PhotoVs.Logic.Debugger;
using PhotoVs.Logic.Events;
using PhotoVs.Logic.Mechanics.Camera.Systems;
using PhotoVs.Logic.Mechanics.Input.Systems;
using PhotoVs.Logic.Modules;
using PhotoVs.Logic.PlayerData;
using PhotoVs.Logic.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using PhotoVs.Engine.Graphics.Filters;
using PhotoVs.Engine.TiledMaps.Layers;
using PhotoVs.Logic.Mechanics.World;
using PhotoVs.Logic.NewScenes;
using Color = Microsoft.Xna.Framework.Color;

namespace PhotoVs.Logic
{
    public class MainGame : Game
    {
        private readonly IPlatform _platform;

        private readonly EventQueue<GameEvents> _events;
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private readonly Services _services;
        private IAssetLoader _assetLoader;
        private SCamera _camera;
        private DiagnosticInfo _info;
        private Player _player;
        private Renderer _renderer;
        private SceneMachine _sceneMachine;
        private SpriteBatch _spriteBatch;
        private CoroutineRunner _coroutineRunner;
        private Overworld _world;

        private ScriptHost _scriptHost;

        public MainGame(IPlatform platform)
        {
            IsMouseVisible = true;
            Window.AllowUserResizing = true;

            _services = new Services();
            _events = new EventQueue<GameEvents>();
            _coroutineRunner = new CoroutineRunner();
            _services.Set(_events);
            _services.Set(_coroutineRunner);

            _platform = platform;
            _services.Set(platform);

            _graphicsDeviceManager = new GraphicsDeviceManager(this)
            {
                GraphicsProfile = GraphicsProfile.HiDef,
                SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight
            };
            if (_platform.OverrideFullscreen)
            {
                _graphicsDeviceManager.PreferredBackBufferWidth = Window.ClientBounds.Width;
                _graphicsDeviceManager.PreferredBackBufferHeight = Window.ClientBounds.Height;
                _graphicsDeviceManager.IsFullScreen = true;
            }
            else
            {
                _graphicsDeviceManager.PreferredBackBufferWidth = 640 * 2;
                _graphicsDeviceManager.PreferredBackBufferHeight = 360 * 2;
            }
            _graphicsDeviceManager.ApplyChanges();
            _services.Set(_graphicsDeviceManager);
            _services.Set(Window);

            // use screen refresh rate
            _graphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
            IsFixedTimeStep = false;
        }

        protected override void Initialize()
        {
            _services.Set(GraphicsDevice);

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _services.Set(_spriteBatch);

            _services.Set(Config.Load());

            _assetLoader = CreateAssetLoader();
            _services.Set(_assetLoader);

            _renderer = CreateRenderer();
            _services.Set(_renderer);

            _player = new Player(_services);
            _services.Set(_player);

            _camera = CreateCamera();
            _services.Set(_camera);

            _services.Set(CreateTextDatabase());

            _sceneMachine = CreateSceneMachine();
            _services.Set(_sceneMachine);

            _services.Set(CreateAudio());

            _info = new DiagnosticInfo(this, _spriteBatch, _assetLoader);

            if (_services.Get<Config>().Fullscreen)
                EnableFullscreen();

            _scriptHost = new ScriptHost(_services,
                new List<(DataLocation, string)>()
                {
                    (DataLocation.Content, "logic/"),
                    (DataLocation.Storage, "mods/")
                },
                new List<Module>
                {
                    new StandardLibraryModule(), // <--- THIS MUST ALWAYS GO FIRST.

                    new EventConditionsModule(_services.Get<Player>()),
                    new EventTriggersModule(_services.Get<EventQueue<GameEvents>>(), _services.Get<Player>()),
                    new SceneMachineModule(_services),
                    new TimingModule(),
                    new DialogueModule(_services.Get<SceneMachine>()),
                    new PlayerModule(_services.Get<Player>()),
                    new GameObjectModule(_services.Get<SceneMachine>(), _services.Get<Player>()),
                    new TextModule(_services.Get<TextDatabase>())
                });

            _world = new Overworld(_spriteBatch, _assetLoader);
            _world.LoadMaps("maps/");
            _world.SetMap("test");
            _services.Set(_world);

            _services.Get<Player>()
                .PlayerData.Position.Position = new Vector2(2750, 1400);

            _events.Notify(GameEvents.GameStart, new GameEventArgs(this));

            var mapBaker = new MapBaker(_assetLoader, _spriteBatch, _renderer);
            mapBaker.Bake("content/maps/", "content/debug/");


            base.Initialize();
        }

        private IAssetLoader CreateAssetLoader()
        {
            var assetLoader = new AssetLoader(_services, _platform.StreamProvider);
            assetLoader
                .RegisterTypeLoader(new EffectTypeLoader(GraphicsDevice))
                .RegisterTypeLoader(new TextTypeLoader())
                .RegisterTypeLoader(new Texture2DTypeLoader(GraphicsDevice))
                .RegisterTypeLoader(new SpriteFontTypeLoader(GraphicsDevice, assetLoader))
                .RegisterTypeLoader(new DynamicSpriteFontTypeLoader(32))
                .RegisterTypeLoader(new MapTypeLoader());

            return assetLoader;
        }

        private IAudio CreateAudio()
        {
            var audio = new DummyAudio();
            return audio;
        }

        private TextDatabase CreateTextDatabase()
        {
            var textDatabase = new TextDatabase(_services);
            return textDatabase;
        }

        private Renderer CreateRenderer()
        {
            var renderer = new Renderer(_services, 640, 360, 840, 400);

            renderer.AddFilter(
                new FunkyFilter(
                    renderer,
                    _assetLoader.Get<Effect>("shaders/funky.fx"),
                    _assetLoader.Get<Texture2D>("ui/noise3.png"),
                    _assetLoader.Get<Texture2D>("ui/noise4.png"),
                    new Color(251, 246, 63),
                    new Color(222, 31, 152),
                    new Color(1, 124, 213)));

            return renderer;
        }

        private SceneMachine CreateSceneMachine()
        {
            var sceneMachine = new SceneMachine(_player, _renderer, CreateGlobalSystems(), CreateGlobalEntities());
            return sceneMachine;
        }

        private GameObjectList CreateGlobalEntities()
        {
            var globalEntities = new GameObjectList
            {
                _player
            };
            return globalEntities;
        }
        private SystemList CreateGlobalSystems()
        {
            var globalSystems = new SystemList
            {
                _camera,
                new SProcessInputState(),
                new SProcessController(),
                new SProcessKeyboard(),
                /*new SHandleFullscreen(_graphicsDeviceManager, GraphicsDevice),
                new STakeScreenshot(GraphicsDevice, _renderer, _spriteBatch,
                    _assetLoader.Get<SpriteFont>("ui/fonts/bold_outline_12.fnt"),
                        _services.Get<Config>())*/
            };
            return globalSystems;
        }

        private SCamera CreateCamera()
        {
            var camera = new SCamera(_renderer);
            camera.Follow(_player);
            return camera;
        }

        protected override void Update(GameTime gameTime)
        {
            _events.ProcessQueue();

            _info.BeforeUpdate();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _scriptHost.Update(gameTime);
            _coroutineRunner.Update(gameTime);
            _sceneMachine.Update(gameTime);

            base.Update(gameTime);

            _info.AfterUpdate();
        }

        protected override void Draw(GameTime gameTime)
        {
            _info.BeforeDraw();

            GraphicsDevice.Clear(Color.CornflowerBlue);

            _renderer.BeforeDraw();
            _sceneMachine.Draw(gameTime);
            //_sceneMachine.DrawUI(gameTime, _renderer.GetUIOrigin());
            _renderer.Draw(gameTime);

            base.Draw(gameTime);

            _info.AfterDraw();
            _info.Draw(gameTime);
        }

        private void EnableFullscreen()
        {
            _graphicsDeviceManager.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            _graphicsDeviceManager.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            _graphicsDeviceManager.IsFullScreen = true;
            _graphicsDeviceManager.ApplyChanges();
        }
    }
}