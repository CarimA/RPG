using System.Collections;
using System.Collections.Generic;
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
using PhotoVs.Engine.FSM.Scenes;
using PhotoVs.Engine.Graphics;
using PhotoVs.Engine.Plugins;
using PhotoVs.Engine.Scheduler;
using PhotoVs.Engine.Scheduler.YieldInstructions;
using PhotoVs.Logic.Camera;
using PhotoVs.Logic.Debug;
using PhotoVs.Logic.Input;
using PhotoVs.Logic.PlayerData;
using PhotoVs.Logic.Scenes;
using PhotoVs.Logic.Text;
using PhotoVs.Models.Assets;
using PhotoVs.Models.Audio;
using PhotoVs.Models.FSM;
using PhotoVs.Utils.Logging;

namespace PhotoVs.Logic
{
    public class MainGame : Game
    {
        private DiagnosticInfo _info;
        private readonly GraphicsDeviceManager _graphics;

        private IAssetLoader _assetLoader;
        private IAudio _audio;
        private SCamera _camera;

        private Database _database;
        private Events _events;
        private Coroutines _coroutines;
        private GameObjectCollection _globalEntities;
        private SystemCollection _globalSystems;

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

            _events = new Events();
            _coroutines = new Coroutines();
            _plugins = new PluginProvider("assets/plugins/", _events, _coroutines);

            _assetLoader = new DebugHotReloadAssetLoader(new FileSystemStreamProvider("assets/"));
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
            _globalEntities = new GameObjectCollection
            {
                _player
            };

            _globalSystems = new SystemCollection
            {
                _camera,
                new SProcessInput(),
                new SHandleFullscreen(_graphics, GraphicsDevice),
                new STakeScreenshot(GraphicsDevice)
            };

            _camera.Follow(_player);

            _sceneMachine = new SceneMachine(_spriteBatch, _assetLoader, _events, _camera, _player);
            _sceneMachine.ChangeToOverworldScene();
            _sceneManager = new SceneManager(_sceneMachine, _globalSystems, _globalEntities);

            _database = new Database(_assetLoader, _player);

            _audio = new DummyAudio();

            _info = new DiagnosticInfo(_spriteBatch, _assetLoader);

            _events.RaiseOnGameStart();

            _coroutines.Start(Test());
            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            _info.BeforeUpdate();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _coroutines.Update(gameTime);
            _sceneManager.Update(gameTime);

            base.Update(gameTime);

            _info.AfterUpdate();
        }

        protected override void Draw(GameTime gameTime)
        {
            _info.BeforeDraw();

            //GraphicsDevice.Clear(Color.CornflowerBlue);

            _renderer.SetRenderMode(RenderMode.Game);
            _sceneManager.Draw(gameTime);
            _renderer.Draw(_spriteBatch);

            base.Draw(gameTime);

            _info.AfterDraw();
            _info.Draw(gameTime);
        }

        private IEnumerator Test()
        {
            Utils.Logging.Debug.Log.Trace("1");
            yield return null;
            Utils.Logging.Debug.Log.Trace("2");
            yield return new Pause(10f);
            _sceneMachine.PushDialogueScene("test", "hello!");
        }
    }
}