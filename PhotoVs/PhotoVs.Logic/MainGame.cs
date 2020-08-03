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
using PhotoVs.Engine.Graphics;
using PhotoVs.Engine.Scripting;
using PhotoVs.Logic.Debugger;
using PhotoVs.Logic.Mechanics.World;
using PhotoVs.Logic.Modules;
using PhotoVs.Logic.NewScenes;
using PhotoVs.Logic.Text;

namespace PhotoVs.Logic
{
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
                .Bind(new SpriteBatch(GraphicsDevice))
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
                .Bind<ICanvasSize, TargetCanvasSize>()
                .Bind<ICoroutineRunner, CoroutineRunner>()
                .Bind<IRenderer, Renderer>()
                .Bind<IAudio>(new DebugAudio(_platform.Audio)) // _platform.Audio))
                .Bind<IScriptHost, ScriptHost<Closure>>()
                .Bind<IInterpreter<Closure>, MoonSharpInterpreter>()
                .Bind<DiagnosticInfo>()

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
                .Bind(new VirtualGameSize(640, 360, 360)) //720))
                .Bind(new ScriptData(new List<(DataLocation, string)>
                {
                    (DataLocation.Content, "logic/"),
                    (DataLocation.Storage, "mods/")
                }));

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

            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            _kernel.Dispose();
            base.OnExiting(sender, args);
        }
    }
}