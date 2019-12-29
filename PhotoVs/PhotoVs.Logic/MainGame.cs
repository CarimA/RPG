using System;
using System.Collections;
using System.IO;
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
using PhotoVs.Engine.Scheduler;
using PhotoVs.Engine.Scheduler.YieldInstructions;
using PhotoVs.Logic.Camera;
using PhotoVs.Logic.Debug;
using PhotoVs.Logic.Input;
using PhotoVs.Logic.PlayerData;
using PhotoVs.Logic.Plugins;
using PhotoVs.Logic.Scenes;
using PhotoVs.Logic.Services;
using PhotoVs.Logic.Text;
using PhotoVs.Models.Assets;
using PhotoVs.Models.Audio;
using PhotoVs.Models.ECS;
using PhotoVs.Models.FSM;
using PhotoVs.Models.Text;
using PhotoVs.Utils.Logging;

namespace PhotoVs.Logic
{
    public class MainGame : Game
    {
        private readonly ServiceLocator _services;
        private DiagnosticInfo _info;

        public MainGame()
        {
            IsMouseVisible = true;
            Window.AllowUserResizing = true;

            var myDocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            Directory.CreateDirectory(Path.Combine(myDocs, "PhotoVs"));
            Directory.CreateDirectory(Path.Combine(myDocs, "PhotoVs/Saves"));
            Directory.CreateDirectory(Path.Combine(myDocs, "PhotoVs/Saves/Save1"));
            Directory.CreateDirectory(Path.Combine(myDocs, "PhotoVs/Saves/Save2"));
            Directory.CreateDirectory(Path.Combine(myDocs, "PhotoVs/Saves/Save3"));
            Directory.CreateDirectory(Path.Combine(myDocs, "PhotoVs/Mods"));
            Directory.CreateDirectory(Path.Combine(myDocs, "PhotoVs/Options"));
            Directory.CreateDirectory(Path.Combine(myDocs, "PhotoVs/Screenshots"));
            Directory.CreateDirectory(Path.Combine(myDocs, "PhotoVs/Logs"));

            Logger.Write.Trace("Creating [My Documents]/PhotoVs");

            _services = new ServiceLocator(new Events());
            _services.Set(new GraphicsDeviceManager(this)
            {
                GraphicsProfile = GraphicsProfile.HiDef,
                PreferredBackBufferWidth = 320,
                PreferredBackBufferHeight = 180
            });
        }

        protected override void Initialize()
        {
            _services.Set(GraphicsDevice);
            _services.Set(new SpriteBatch(GraphicsDevice));
            _services.Set(new Coroutines());
            _services.Set(new PluginProvider(_services));
            _services.Plugins.LoadPlugins("assets/plugins/");
            _services.Plugins.LoadPlugins(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "PhotoVs/Mods"));
            _services.Plugins.LoadPlugin(typeof(TestPlugin));

            _services.Set(CreateAssetLoader());
            _services.Set(CreateRenderer());
            _services.Set(new Player());
            _services.Set(CreateCamera());
            _services.Set(CreateGlobalEntities());
            _services.Set(CreateGlobalSystems());
            _services.Set(CreateSceneMachine());
            _services.Set<ISceneManager>(new SceneManager(_services.SceneMachine,
                _services.GlobalSystems,
                _services.GlobalGameObjects));
            _services.Set(CreateTextDatabase());
            _services.Set(CreateAudio());

            _info = new DiagnosticInfo(_services.SpriteBatch, _services.AssetLoader);
            _services.Events.RaiseOnGameStart();


            base.Initialize();
        }

        private IAssetLoader CreateAssetLoader()
        {
            var assetLoader = new HotReloadAssetLoader(_services.Coroutines, new FileSystemStreamProvider("assets/"));
            assetLoader
                .RegisterTypeLoader(new EffectTypeLoader(_services.GraphicsDevice))
                .RegisterTypeLoader(new TextTypeLoader())
                .RegisterTypeLoader(new Texture2DTypeLoader(_services.GraphicsDevice))
                .RegisterTypeLoader(new BitmapFontTypeLoader(assetLoader))
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
            var textDatabase = new TextDatabase(_services.AssetLoader, _services.Player);
            return textDatabase;
        }

        private Renderer CreateRenderer()
        {
            var canvas = new CanvasSize(320, 180);
            var renderer = new Renderer(GraphicsDevice,
                _services.GraphicsDeviceManager,
                Window,
                new ColorGrading(GraphicsDevice,
                    canvas,
                    _services.AssetLoader.GetAsset<Effect>("colorgrading/color.dx11"),
                    _services.AssetLoader.GetAsset<Texture2D>("colorgrading/aap128.png")),
                canvas);
            return renderer;
        }

        private IGameObjectCollection CreateGlobalEntities()
        {
            var globalEntities = new GameObjectCollection
            {
                _services.Player
            };
            return globalEntities;
        }

        private SCamera CreateCamera()
        {
            var camera = new SCamera(_services.Renderer);
            camera.Follow(_services.Player);
            return camera;
        }

        private ISystemCollection CreateGlobalSystems()
        {
            var globalSystems = new SystemCollection
            {
                _services.Camera,
                new SProcessInput(),
                new SHandleFullscreen(_services.GraphicsDeviceManager, GraphicsDevice),
                new STakeScreenshot(GraphicsDevice)
            };
            return globalSystems;
        }

        private SceneMachine CreateSceneMachine()
        {
            var sceneMachine = new SceneMachine(_services);
            sceneMachine.ChangeToOverworldScene();
            return sceneMachine;
        }

        protected override void Update(GameTime gameTime)
        {
            _info.BeforeUpdate();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _services.Coroutines.Update(gameTime);
            _services.SceneManager.Update(gameTime);

            base.Update(gameTime);

            _info.AfterUpdate();
        }

        protected override void Draw(GameTime gameTime)
        {
            _info.BeforeDraw();

            GraphicsDevice.Clear(Color.CornflowerBlue);

            _services.Renderer.SetRenderMode(RenderMode.Game);
            _services.SceneManager.Draw(gameTime);
            _services.Renderer.Draw(_services.SpriteBatch);

            base.Draw(gameTime);

            _info.AfterDraw();
            _info.Draw(gameTime);
        }

        public class TestPlugin : Plugin
        {
            private ITextDatabase _db;
            public override string Name { get; } = "Test Plugin";
            public override string Version { get; } = "1.0.0";

            public override void Bind(Events events)
            {
                events.OnInteractEventEnter["example_event"] += InteractEventHandler;
                events.OnServiceSet[typeof(ITextDatabase)] += (sender, type) => { _db = (ITextDatabase) type; };
            }

            private void InteractEventHandler(object sender, IGameObject player, IGameObject script)
            {
                Spawn(LockMovement(DoThis));
            }

            private IEnumerator DoThis()
            {
                var text = TextInput("Hi", "Test", 10);
                yield return text;

                yield return Spawn(WaitTest());
                Logger.Write.Trace("Test 1");

                yield return Dialogue("test", $"Hello {text.Text}!");
                yield return Dialogue("test", _db.GetText("Intro"));
                Logger.Write.Trace("Test 2");
                yield return new Pause(3f);
                Logger.Write.Trace("Test 3");
            }

            private IEnumerator WaitTest()
            {
                yield return new Pause(3f);
            }
        }
    }
}