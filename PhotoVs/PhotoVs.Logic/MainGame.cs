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
using PhotoVs.Engine.ECS;
using PhotoVs.Engine.Events.Coroutines;
using PhotoVs.Engine.Graphics;
using PhotoVs.Engine.Scripting;
using PhotoVs.Logic.BuildTools;
using PhotoVs.Logic.Debugger;
using PhotoVs.Logic.Mechanics;
using PhotoVs.Logic.Mechanics.World;
using PhotoVs.Logic.Modules;
using PhotoVs.Logic.Scenes;
using PhotoVs.Logic.Text;
using GameObject = PhotoVs.Logic.Modules.GameObject;

namespace PhotoVs.Logic
{
    public class MainGame : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private readonly Kernel _kernel;
        private readonly IPlatform _platform;
        private readonly Scheduler _scheduler;
        private SpriteBatch _spriteBatch;

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
            // todo: maybe have a BindModule<T, TProxy>() that integrates with scripting?

            _kernel
                // monogame specific things
                .Bind<Game>(this)
                .Bind(_graphics)
                .Bind(GraphicsDevice)
                .Bind(_spriteBatch = new SpriteBatch(GraphicsDevice))
                .Bind(Window)

                // type loaders for assets
                // this should come before in-case if anything wants to load assets
                .Bind<EffectTypeLoader>()
                .Bind<TextTypeLoader>()
                .Bind<Texture2DTypeLoader>()
                .Bind<SpriteFontTypeLoader>()
                .Bind<DynamicSpriteFontTypeLoader>()
                .Bind<MapTypeLoader>()
                .Bind<IAssetLoader, AssetLoader>()

                // core engine specific things
                .Bind(_platform)
                .Bind<ISignal, Signal>()
                .Bind<FullscreenHandler>()
                .Bind<ScreenshotHandler>()
                .Bind<CanvasSize>()
                .Bind<ICoroutineRunner, CoroutineRunner>()
                .Bind<IRenderer, Renderer>()
                .Bind<Primitive>()
                .Bind<IAudio>(new DebugAudio(_platform.Audio)) // _platform.Audio))
                .Bind<IScriptHost, ScriptHost<Closure>>()
                .Bind<IInterpreter<Closure>, MoonSharpInterpreter>()
                .Bind<DiagnosticInfo>()

                // game logic
                .Bind<Random>(new Random())
                .Bind<GameState>()
                .Bind<ITextDatabase, TextDatabase>()
                .Bind<IOverworld, Overworld>()
                .Bind<GameDate>()

                // systems
                .Bind<Stage>()
                .Bind<GlobalSystems>()
                .Bind<Camera>()
                .Bind<Input>()
                .Bind<DrawMap>()
                .Bind<Movement>()
                .Bind<Animation>()

                // scenes
                .Bind<Test>()

                // modules for scripting
                .Bind<StandardLibrary>()
                .Bind<EventConditions>()
                .Bind<EventTriggers>()
                .Bind<Timing>()
                .Bind<Player>()
                .Bind<GameObject>()
                .Bind<Modules.Text>()

                // data that will get used to initialise state
                .Bind(new VirtualResolution(640, 360))
                .Bind(new ScriptData(new List<(DataLocation, string)>
                {
                    (DataLocation.Content, "logic/"),
                    (DataLocation.Storage, "mods/")
                }));

            _kernel.Construct();
            _scheduler.Start();

            var mapBaker2 = new MapBaker2((Kernel)_kernel, "content/maps/", "content/debug/", 16);
            //mapBaker2.Bake();

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

            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            _kernel.Dispose();
            base.OnExiting(sender, args);
        }
    }
}