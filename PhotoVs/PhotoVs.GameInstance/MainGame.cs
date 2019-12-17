using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PhotoVs.Assets.AssetLoaders;
using PhotoVs.Assets.StreamProviders;
using PhotoVs.Assets.TypeLoaders;
using PhotoVs.Audio;
using PhotoVs.CommonGameLogic.Camera;
using PhotoVs.CommonGameLogic.Input;
using PhotoVs.CommonGameLogic.Scenes;
using PhotoVs.ECS.Entities;
using PhotoVs.ECS.Systems;
using PhotoVs.Events;
using PhotoVs.FSM.Scenes;
using PhotoVs.Graphics;
using PhotoVs.Logs;
using PhotoVs.PlayerData;
using PhotoVs.Plugins;
using PhotoVs.Text;

namespace PhotoVs.GameInstance
{
    public class MainGame : Game
    {
        private readonly GraphicsDeviceManager _graphics;

        private IAssetLoader _assetLoader;
        private IAudio _audio;
        private SCamera _camera;

        private Database _database;
        private GameEvents _gameEvents;
        private EntityCollection _globalEntities;
        private SystemCollection _globalSystems;

        private LoggerCollection _logger;

        private Player _player;
        private PluginProvider _plugins;

        private Renderer _renderer;
        private SceneMachine _sceneMachine;
        private ISceneManager _sceneManager;
        private SpriteBatch _spriteBatch;

        public MainGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;

            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;

            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _logger = new LoggerCollection
            {
                new ConsoleLogger(LogLevel.Trace)
            };

            _gameEvents = new GameEvents();
            _plugins = new PluginProvider("plugins/", _gameEvents);

            _assetLoader = new DebugHotReloadAssetLoader(_logger, new FileSystemStreamProvider("assets/"));
            _assetLoader
                .RegisterTypeLoader(new EffectTypeLoader(GraphicsDevice))
                .RegisterTypeLoader(new TextTypeLoader())
                .RegisterTypeLoader(new Texture2DTypeLoader(GraphicsDevice))
                .RegisterTypeLoader(new BitmapFontTypeLoader(_assetLoader))
                .RegisterTypeLoader(new MapTypeLoader());

            var canvas = new CanvasSize(320, 180);
            _renderer = new Renderer(GraphicsDevice,
                _graphics,
                Window,
                new ColorGrading(GraphicsDevice, canvas, _assetLoader.GetAsset<Effect>("colorgrading/color.dx11"),
                    _assetLoader.GetAsset<Texture2D>("colorgrading/main.png")),
                canvas);
            _camera = new SCamera(_renderer);

            _player = new Player();
            _globalEntities = new EntityCollection
            {
                _player
            };

            _globalSystems = new SystemCollection
            {
                _camera,
                new SProcessInput()
            };

            _camera.Follow(_player);

            _sceneMachine = new SceneMachine(_spriteBatch, _assetLoader, _gameEvents, _camera);
            _sceneMachine.ChangeToOverworldScene();
            _sceneManager = new SceneManager(_sceneMachine, _globalSystems, _globalEntities);

            _database = new Database(_assetLoader, _player);

            _audio = new DummyAudio();

            _gameEvents.Raise("game:start");
            base.Initialize();
        }

        protected override void LoadContent()
        {
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _sceneManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _renderer.SetRenderMode(RenderMode.Game);
            _sceneManager.Draw(gameTime);
            _renderer.Draw(_spriteBatch);

            base.Draw(gameTime);
        }
    }
}