using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Assets.AssetLoaders;
using PhotoVs.CommonGameLogic.Camera;
using PhotoVs.Events;
using PhotoVs.FSM.States;
using PhotoVs.GameInstance;

namespace PhotoVs.FSM.Scenes
{
    public class SceneMachine : StateMachine<IScene>
    {
        private readonly OverworldScene _overworldScene;

        public SpriteBatch SpriteBatch { get; }
        public IAssetLoader AssetLoader { get; }
        public SCamera Camera { get; }
        public GameEvents GameEvents { get; }

        public SceneMachine(SpriteBatch spriteBatch, IAssetLoader assetLoader, GameEvents gameEvents, SCamera camera)
        {
            SpriteBatch = spriteBatch;
            AssetLoader = assetLoader;
            GameEvents = gameEvents;
            Camera = camera;

            _overworldScene = new OverworldScene(this);
        }
        public void ChangeToOverworldScene()
        {
            Change(_overworldScene);
        }
    }
}
