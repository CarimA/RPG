using System;
using Microsoft.Xna.Framework.Graphics;
using MoonSharp.Interpreter;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.Core;
using PhotoVs.Engine.Graphics;
using PhotoVs.Engine.Scripting;
using PhotoVs.Logic.Mechanics.World;
using PhotoVs.Logic.NewScenes;
using PhotoVs.Logic.NewScenes.GameScenes;

namespace PhotoVs.Logic.Modules
{
    public class SceneMachineModule
    {
        private readonly IAssetLoader _assetLoader;
        private readonly ICanvasSize _canvasSize;
        private readonly IGameState _gameState;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly IOverworld _overworld;
        private readonly IRenderer _renderer;
        private readonly SceneMachine _sceneMachine;
        private readonly ISignal _signal;
        private readonly SpriteBatch _spriteBatch;

        public SceneMachineModule(IInterpreter<Closure> interpreter, SceneMachine sceneMachine,
            IAssetLoader assetLoader, IRenderer renderer, IOverworld overworld, SpriteBatch spriteBatch,
            IGameState gameState,
            ISignal signal, GraphicsDevice graphicsDevice, ICanvasSize canvasSize)
        {
            _sceneMachine = sceneMachine;
            _assetLoader = assetLoader;
            _renderer = renderer;
            _overworld = overworld;
            _spriteBatch = spriteBatch;
            _gameState = gameState;
            _signal = signal;
            _graphicsDevice = graphicsDevice;
            _canvasSize = canvasSize;

            interpreter.AddFunction("PushScene", (Action<string>) PushScene);
        }

        private void PushScene(string sceneName)
        {
            switch (sceneName)
            {
                case "controller":
                    //_sceneMachine.Push(new ControllerRecommendationScreen(_sceneMachine));
                    _sceneMachine.Push(new WorldScene(_assetLoader, _renderer, _overworld, _spriteBatch, _gameState,
                        _signal, _graphicsDevice, _canvasSize));
                    //_sceneMachine.Push(new TitleScene(_services));
                    _sceneMachine.Push(new WorldLogicScene(_gameState, _assetLoader, _spriteBatch, _overworld,
                        _signal));
                    break;

                default:
                    throw new ArgumentException(nameof(sceneName));
            }
        }
    }
}