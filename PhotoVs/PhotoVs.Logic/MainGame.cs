﻿using Microsoft.Xna.Framework;
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
using PhotoVs.Logic.Scenes;
using PhotoVs.Logic.Text;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Color = Microsoft.Xna.Framework.Color;

namespace PhotoVs.Logic
{
    public class MainGame : Game
    {
        private readonly IPlatform _platform;

        private readonly Engine.Events.EventQueue<GameEvents> _events;
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

            _services.Set(CreateGlobalEntities());

            _services.Set(CreateGlobalSystems());

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
                    new EventConditionsModule(_services.Get<Player>()),
                    new EventTriggersModule(_services.Get<EventQueue<GameEvents>>()),
                    new SceneMachineModule(_services.Get<SceneMachine>()),
                    new TimingModule(),
                    new DialogueModule(_services.Get<SceneMachine>()),
                    new PlayerModule(_services.Get<Player>()),
                    new GameObjectModule(_services.Get<GameObjectList>(), _services.Get<Player>()),
                    new TextModule(_services.Get<TextDatabase>())
                });

            _events.Notify(GameEvents.GameStart, new GameEventArgs(this));

            PostprocessMap("albion.tmx", "albion-out.tmx");

            base.Initialize();
        }

        private void PostprocessMap(string inputFile, string outputFile)
        {
            // todo: turn this postprocess step into a command line tool
            var convertMap = _assetLoader.Get<Map>(inputFile);
            TmxMap.CompressLayers(convertMap, _assetLoader);

            using var filestream = new FileStream(outputFile, FileMode.Create);
            using var writer = new XmlTextWriter(filestream, Encoding.UTF8);
            writer.WriteWholeMap(convertMap);
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
            return renderer;
        }

        private GameObjectList CreateGlobalEntities()
        {
            var globalEntities = new GameObjectList
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

        private ISystemCollection<ISystem> CreateGlobalSystems()
        {
            var globalSystems = new SystemCollection<ISystem>
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

        private SceneMachine CreateSceneMachine()
        {
            var sceneMachine = new SceneMachine(_services);
            return sceneMachine;
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
            _sceneMachine.DrawUI(gameTime, _renderer.GetUIOrigin());
            _renderer.Draw();

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