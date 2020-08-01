using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.Audio;
using PhotoVs.Engine.Core;
using PhotoVs.Engine.Events.EventArgs;
using PhotoVs.Engine.Graphics;
using PhotoVs.Logic.Mechanics.World;
using PhotoVs.Logic.NewScenes;
using PhotoVs.Logic.NewScenes.GameScenes;

namespace PhotoVs.Logic
{
    public class StartupSequence : IStartup
    {
        private readonly IGameState _gameState;
        private readonly ISignal _signal;
        private readonly IOverworld _overworld;
        private readonly SceneMachine _sceneMachine;
        private readonly IAudio _audio;
        private readonly IAssetLoader _assetLoader;
        private readonly IRenderer _renderer;
        private readonly SpriteBatch _spriteBatch;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly ICanvasSize _canvasSize;

        public StartupSequence(IGameState gameState, ISignal signal, IOverworld overworld, SceneMachine sceneMachine, IAudio audio, 
            IAssetLoader assetLoader, IRenderer renderer, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, ICanvasSize canvasSize)
        {
            _gameState = gameState;
            _signal = signal;
            _overworld = overworld;
            _sceneMachine = sceneMachine;
            _audio = audio;
            _assetLoader = assetLoader;
            _renderer = renderer;
            _spriteBatch = spriteBatch;
            _graphicsDevice = graphicsDevice;
            _canvasSize = canvasSize;
        }

        public void Start()
        {
            _overworld.LoadMaps("maps/");
            _overworld.SetMap("novalondinium");

            _gameState.Player.PlayerData.Position.Position = new Vector2(8400, 6000);

            _sceneMachine.Push(new WorldScene(_assetLoader, _renderer, _overworld, _spriteBatch, _gameState, _signal,
                _graphicsDevice, _canvasSize));
            //_sceneMachine.Push(new TitleScene(_services));
            _sceneMachine.Push(new WorldLogicScene(_gameState, _assetLoader, _spriteBatch, _overworld, _signal));

            _signal.Notify("GameStart", new GameEventArgs(this));

            _audio.PlayBgm("key");
        }
    }
}