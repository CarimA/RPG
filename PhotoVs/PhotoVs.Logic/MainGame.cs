﻿using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PhotoVs.Engine;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.Assets.StreamProviders;
using PhotoVs.Engine.Assets.TypeLoaders;
using PhotoVs.Engine.Audio;
using PhotoVs.Engine.ECS.GameObjects;
using PhotoVs.Engine.ECS.Systems;
using PhotoVs.Engine.Graphics;
using PhotoVs.Engine.Scheduler;
using PhotoVs.Engine.Scheduler.YieldInstructions;
using PhotoVs.Logic.Camera;
using PhotoVs.Logic.Debug;
using PhotoVs.Logic.Input;
using PhotoVs.Logic.PlayerData;
using PhotoVs.Logic.Plugins;
using PhotoVs.Logic.Scenes;
using PhotoVs.Logic.Text;
using PhotoVs.Models.Assets;
using PhotoVs.Models.Audio;
using PhotoVs.Models.ECS;
using PhotoVs.Models.Text;
using PhotoVs.Utils.Logging;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace PhotoVs.Logic
{
    public class MainGame : Game
    {
        private readonly Services _services;
        private DiagnosticInfo _info;

        private readonly Events _events;
        private Coroutines _coroutines;
        private PluginProvider _pluginProvider;
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private SceneMachine _sceneMachine;
        private SCamera _camera;
        private Player _player;
        private Renderer _renderer;
        private IAssetLoader _assetLoader;
        private SpriteBatch _spriteBatch;

        public MainGame()
        {
            IsMouseVisible = true;
            Window.AllowUserResizing = true;

            // todo: figure out how to abstract this away
            var myDocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            Directory.CreateDirectory(Path.Combine(myDocs, "PhotoVs"));
            Directory.CreateDirectory(Path.Combine(myDocs, "PhotoVs/Saves"));
            Directory.CreateDirectory(Path.Combine(myDocs, "PhotoVs/Saves/Save1"));
            Directory.CreateDirectory(Path.Combine(myDocs, "PhotoVs/Saves/Save2"));
            Directory.CreateDirectory(Path.Combine(myDocs, "PhotoVs/Saves/Save3"));
            Directory.CreateDirectory(Path.Combine(myDocs, "PhotoVs/Mods"));
            Directory.CreateDirectory(Path.Combine(myDocs, "PhotoVs/Screenshots"));
            Directory.CreateDirectory(Path.Combine(myDocs, "PhotoVs/Logs"));

            Logger.Write.Trace("Creating [My Documents]/PhotoVs");

            _services = new Services();
            _events = new Events();
            _services.Set(_events);
            _graphicsDeviceManager = new GraphicsDeviceManager(this)
            {
                GraphicsProfile = GraphicsProfile.HiDef,
                PreferredBackBufferWidth = 320,
                PreferredBackBufferHeight = 180
            };
            _services.Set(_graphicsDeviceManager);
        }

        protected override void Initialize()
        {
            _services.Set(GraphicsDevice);

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _services.Set(_spriteBatch);

            _coroutines = new Coroutines();
            _services.Set(_coroutines);

            _services.Set(Config.Load());

            _assetLoader = CreateAssetLoader();
            _services.Set(_assetLoader);

            _renderer = CreateRenderer();
            _services.Set(_renderer);

            _player = new Player(_services);
            _services.Set(_player);

            _camera = CreateCamera();
            _services.Set(_camera);

            _services.Set(CreateGlobalEntities());

            _services.Set(CreateGlobalSystems());

            _services.Set(CreateTextDatabase());

            _sceneMachine = CreateSceneMachine();
            _services.Set(_sceneMachine);

            _services.Set(CreateAudio());

            _info = new DiagnosticInfo(_spriteBatch, _assetLoader);

            if (_services.Get<Config>().Fullscreen)
            {
                EnableFullscreen();
            }

            _pluginProvider = new PluginProvider(_services);
            _services.Set(_pluginProvider);
            _pluginProvider.LoadPlugins("assets/plugins/");
            _pluginProvider.LoadPlugins(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "PhotoVs/Mods"));
            _pluginProvider.LoadPlugin(typeof(TestPlugin));
            _pluginProvider.LoadPlugin(typeof(GameLoad));

            _events.RaiseOnGameStart();

            base.Initialize();
        }

        private IAssetLoader CreateAssetLoader()
        {
            var assetLoader = new HotReloadAssetLoader(_coroutines, new FileSystemStreamProvider("assets/"));
            assetLoader
                .RegisterTypeLoader(new EffectTypeLoader(GraphicsDevice))
                .RegisterTypeLoader(new TextTypeLoader())
                .RegisterTypeLoader(new Texture2DTypeLoader(GraphicsDevice))
                .RegisterTypeLoader(new SpriteFontTypeLoader(GraphicsDevice, assetLoader))
                .RegisterTypeLoader(new DynamicSpriteFontTypeLoader(GraphicsDevice, _assetLoader, 32))
                .RegisterTypeLoader(new MapTypeLoader());

            return assetLoader;
        }

        private IAudio CreateAudio()
        {
            var audio = new DummyAudio();
            return audio;
        }

        private ITextDatabase CreateTextDatabase()
        {
            var textDatabase = new TextDatabase(_services);
            return textDatabase;
        }

        private Renderer CreateRenderer()
        {
            var canvas = new CanvasSize(320, 180);
            var renderer = new Renderer(GraphicsDevice,
                _graphicsDeviceManager,
                Window,
                new ColorGrading(GraphicsDevice,
                    canvas,
                    _assetLoader.GetAsset<Effect>("colorgrading/color.dx11"),
                    _assetLoader.GetAsset<Texture2D>("colorgrading/aap128.png")),
                canvas);
            return renderer;
        }

        private IGameObjectCollection CreateGlobalEntities()
        {
            var globalEntities = new GameObjectCollection
            {
                _player
            };
            return globalEntities;
        }

        private SCamera CreateCamera()
        {
            var camera = new SCamera(_renderer);
            camera.Follow(_player);
            return camera;
        }

        private ISystemCollection CreateGlobalSystems()
        {
            var globalSystems = new SystemCollection
            {
                _camera,
                new SProcessInput(),
                new SHandleFullscreen(_graphicsDeviceManager, GraphicsDevice),
                new STakeScreenshot(GraphicsDevice, _renderer, _spriteBatch,
                    _assetLoader.GetAsset<SpriteFont>("fonts/mono.fnt"))
            };
            return globalSystems;
        }

        private SceneMachine CreateSceneMachine()
        {
            var sceneMachine = new SceneMachine(_services);
            return sceneMachine;
        }

        protected override void Update(GameTime gameTime)
        {
            _info.BeforeUpdate();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _coroutines.Update(gameTime);
            _sceneMachine.Update(gameTime);

            base.Update(gameTime);

            _info.AfterUpdate();
        }

        protected override void Draw(GameTime gameTime)
        {
            _info.BeforeDraw();

            GraphicsDevice.Clear(Color.CornflowerBlue);

            _renderer.SetRenderMode(RenderMode.Game);
            _sceneMachine.Draw(gameTime);

            _renderer.SetRenderMode(RenderMode.UI);
            _sceneMachine.DrawUI(gameTime);

            _renderer.Draw(_spriteBatch);

            base.Draw(gameTime);

            _info.AfterDraw();
            _info.Draw(gameTime);
        }

        public class GameLoad : Plugin
        {
            public override string Name { get; }
            public override string Version { get; }

            private SceneMachine _sceneMachine;

            public override void Bind(Services services)
            {
                _sceneMachine = services.Get<SceneMachine>();

                var events = services.Get<Events>();
                events.OnGameStart += EventsOnOnGameStart;
            }

            private void EventsOnOnGameStart(object sender)
            {
                _sceneMachine.Push(_sceneMachine.ControllerRecommendationScreen);
            }
        }

        public class TestPlugin : Plugin
        {
            private ITextDatabase _db;
            private SCamera _camera;
            private Player _player;

            public override string Name { get; } = "Test Plugin";
            public override string Version { get; } = "1.0.0";

            public override void Bind(Services services)
            {
                var events = services.Get<Events>();

                events.OnInteractEventEnter["example_event"] += InteractEventHandler;

                _db = services.Get<ITextDatabase>();
                _camera = services.Get<SCamera>();
                _player = services.Get<Player>();
            }

            private void InteractEventHandler(object sender, IGameObject player, IGameObject script)
            {
                Spawn(LockMovement(DoThis));
            }

            private IEnumerator DoThis()
            {
                _camera.Set(new System.Collections.Generic.List<Vector2>()
                {
                    new Vector2(-600, -600),
                    new Vector2(600, 600)
                });

                var text = TextInput("Hi", "Test", 10);
                yield return text;

                yield return Spawn(WaitTest());
                Logger.Write.Trace("Test 1");

                yield return Dialogue("test", $"Hello {text.Text}!");
                yield return Dialogue("test", _db.GetText("Intro"));
                Logger.Write.Trace("Test 2");
                yield return new Pause(3f);
                Logger.Write.Trace("Test 3");

                _camera.Follow(_player);
            }

            private IEnumerator WaitTest()
            {
                yield return new Pause(3f);
            }
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