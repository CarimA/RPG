using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.Audio;
using PhotoVs.Engine.Core;
using PhotoVs.Engine.ECS;
using PhotoVs.Engine.Events.EventArgs;
using PhotoVs.Engine.Graphics;
using PhotoVs.Logic.Mechanics.World;
using PhotoVs.Logic.Scenes;

namespace PhotoVs.Logic
{
    public class StartupSequence : IStartup
    {
        private readonly GameState _gameState;
        private readonly Stage _stage;
        private readonly ISignal _signal;
        private readonly IOverworld _overworld;
        private readonly IAudio _audio;
        private readonly IAssetLoader _assetLoader;
        private readonly IRenderer _renderer;
        private readonly SpriteBatch _spriteBatch;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly CanvasSize _canvasSize;

        public StartupSequence(GameState gameState, Stage stage, ISignal signal, IOverworld overworld, IAudio audio, 
            IAssetLoader assetLoader, IRenderer renderer, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, CanvasSize canvasSize)
        {
            _gameState = gameState;
            _stage = stage;
            _signal = signal;
            _overworld = overworld;
            _audio = audio;
            _assetLoader = assetLoader;
            _renderer = renderer;
            _spriteBatch = spriteBatch;
            _graphicsDevice = graphicsDevice;
            _canvasSize = canvasSize;

            /*_renderer.AddFilter(
                new FunkyFilter(
                    _renderer, _spriteBatch,
                    _assetLoader.Get<Effect>("shaders/funky.fx"),
                    _assetLoader.Get<Texture2D>("ui/noise3.png"),
                    _assetLoader.Get<Texture2D>("ui/noise4.png"),
                    new Color(251, 246, 63),
                    new Color(222, 31, 152),
                    new Color(1, 124, 213)));*/
        }

        public void Start(IEnumerable<object> bindings)
        {
            _overworld.LoadMaps("maps/");
            _overworld.SetMap("novalondinium");

            _gameState.Player.PlayerData.Position.Position = new Vector2(8400, 6000);
            

            _stage.ChangeScene<Test>();
            //_sceneMachine.Push(new WorldScene(_assetLoader, _renderer, _overworld, _spriteBatch, _gameState, _signal,
            //    _graphicsDevice, _canvasSize));
            //_sceneMachine.Push(new TitleScene(_services));
            //_sceneMachine.Push(new WorldLogicScene(_gameState, _assetLoader, _spriteBatch, _overworld, _signal));

            _signal.Notify("GameStart", new GameEventArgs(this));

            //_audio.PlayBgm("key");
        }
    }
}