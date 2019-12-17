using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.FSM.States;
using PhotoVs.Logic.Camera;
using PhotoVs.Models;
using PhotoVs.Models.Assets;
using PhotoVs.Models.FSM;

namespace PhotoVs.Logic.Scenes
{
    public class SceneMachine : StateMachine<IScene>
    {
        private readonly OverworldScene _overworldScene;

        public SceneMachine(SpriteBatch spriteBatch, IAssetLoader assetLoader, Events gameEvents, SCamera camera)
        {
            SpriteBatch = spriteBatch;
            AssetLoader = assetLoader;
            GameEvents = gameEvents;
            Camera = camera;

            _overworldScene = new OverworldScene(this);
        }

        public SpriteBatch SpriteBatch { get; }
        public IAssetLoader AssetLoader { get; }
        public SCamera Camera { get; }
        public Events GameEvents { get; }

        public void ChangeToOverworldScene()
        {
            Change(_overworldScene);
        }
    }
}