using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PhotoVs.Engine;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.Assets.TypeLoaders;
using PhotoVs.Engine.Audio;
using PhotoVs.Engine.ECS.GameObjects;
using PhotoVs.Engine.ECS.Systems;
using PhotoVs.Engine.Graphics;
using PhotoVs.Engine.Plugins;
using PhotoVs.Engine.Scheduler;
using PhotoVs.Logic.Debugger;
using PhotoVs.Logic.Mechanics.Camera.Systems;
using PhotoVs.Logic.Mechanics.Input.Systems;
using PhotoVs.Logic.PlayerData;
using PhotoVs.Logic.Scenes;
using PhotoVs.Logic.Text;
using PhotoVs.Utils.Extensions;
using PhotoVs.Utils.Logging;
using System;
using System.IO;
using PhotoVs.Engine.Events;
using PhotoVs.Logic.Events;

namespace PhotoVs.Logic
{
    public class MainGame : Game
    {
        private readonly IPlatform _platform;

        private readonly EventQueue _events;
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private readonly Services _services;
        private IAssetLoader _assetLoader;
        private SCamera _camera;
        private Coroutines _coroutines;
        private DiagnosticInfo _info;
        private Player _player;
        private PluginProvider _pluginProvider;
        private Renderer _renderer;
        private SceneMachine _sceneMachine;
        private SpriteBatch _spriteBatch;

        public MainGame(IPlatform platform)
        {
            IsMouseVisible = true;
            Window.AllowUserResizing = true;

            _platform = platform;

            _services = new Services();
            _events = new EventQueue();
            _services.Set(_events);

            _graphicsDeviceManager = new GraphicsDeviceManager(this)
            {
                GraphicsProfile = GraphicsProfile.HiDef,
                PreferredBackBufferWidth = 320 * 2,
                PreferredBackBufferHeight = 180 * 2,
                SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight
            };
            if (_platform.OverrideFullscreen)
            {
                _graphicsDeviceManager.IsFullScreen = true;
            }
            _graphicsDeviceManager.ApplyChanges();
            _services.Set(_graphicsDeviceManager);

            // use screen refresh rate
            _graphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
            IsFixedTimeStep = false;
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
                EnableFullscreen();

            _services.Set(new Scheduler(_services));

            _pluginProvider = new PluginProvider(_services);
            _services.Set(_pluginProvider);
            AppDomain.CurrentDomain.GetAssemblies().ForEach(_pluginProvider.LoadPluginFromAssembly);
            _pluginProvider.LoadPlugins("logic/");
            _pluginProvider.LoadMods();

            _events.Notify(EventType.GAME_START, new GameEventArgs(this));

            // todo: turn this postprocess step into a command line tool
            /*var convertMap = _assetLoader.GetAsset<Map>("albion.tmx");
            TmxMap.CompressLayers(convertMap, _assetLoader);

            using (var filestream = new FileStream("test.tmx", FileMode.Create)){
                using (var writer = new XmlTextWriter(filestream, Encoding.UTF8))
                {
                    writer.WriteMapElements(convertMap);
                }
            }*/

            base.Initialize();
        }

        private IAssetLoader CreateAssetLoader()
        {
            var assetLoader = new AssetLoader(_coroutines, _platform.StreamProvider);
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
            var canvas = new CanvasSize(320 * 2, 180 * 2);
            var renderer = new Renderer(GraphicsDevice,
                _graphicsDeviceManager,
                Window,
                new ColorGrading(GraphicsDevice,
                    canvas,
                    _assetLoader.Get<Effect>(_platform.PaletteShader),
                    _assetLoader.Get<Texture2D>("ui/luts/aap128.png")),
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

        private ISystemCollection<ISystem> CreateGlobalSystems()
        {
            var globalSystems = new SystemCollection<ISystem>
            {
                _camera,
                new SProcessInput(),
                new SHandleFullscreen(_graphicsDeviceManager, GraphicsDevice),
                new STakeScreenshot(GraphicsDevice, _renderer, _spriteBatch,
                    _assetLoader.Get<SpriteFont>("ui/fonts/bold_outline_12.fnt"),
                        _services.Get<Config>())
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
            _events.Process();

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
            _sceneMachine.DrawUI(gameTime, _renderer.GetUIOrigin());

            _renderer.Draw(_spriteBatch);

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