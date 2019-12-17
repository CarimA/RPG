using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Assets.AssetLoaders;
using PhotoVs.CommonGameLogic.Camera;
using PhotoVs.Events;
using PhotoVs.FSM.Scenes;
using PhotoVs.FSM.States;

namespace PhotoVs.CommonGameLogic.Scenes
{
    public class SceneMachine : StateMachine<IScene>
    {
        private readonly OverworldScene _overworldScene;

        public SceneMachine(SpriteBatch spriteBatch, IAssetLoader assetLoader, GameEvents gameEvents, SCamera camera)
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
        public GameEvents GameEvents { get; }

        public void ChangeToOverworldScene()
        {
            Change(_overworldScene);
        }
    }
}