using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine;
using PhotoVs.Engine.FSM.States;
using PhotoVs.Logic.Camera;
using PhotoVs.Logic.PlayerData;
using PhotoVs.Models.Assets;
using PhotoVs.Models.FSM;

namespace PhotoVs.Logic.Scenes
{
    public class SceneMachine : StateMachine<IScene>
    {
        private readonly OverworldScene _overworldScene;
        private readonly DialogueScene _dialogueScene;

        public SceneMachine(SpriteBatch spriteBatch, IAssetLoader assetLoader, Events gameEvents, SCamera camera, Player player)
        {
            SpriteBatch = spriteBatch;
            AssetLoader = assetLoader;
            GameEvents = gameEvents;
            Camera = camera;
            Player = player;

            _overworldScene = new OverworldScene(this);
            _dialogueScene = new DialogueScene(this);
        }

        public SpriteBatch SpriteBatch { get; }
        public IAssetLoader AssetLoader { get; }
        public SCamera Camera { get; }
        public Events GameEvents { get; }
        public Player Player { get; }

        public void ChangeToOverworldScene()
        {
            Change(_overworldScene);
        }

        public void PushDialogueScene(string name, string dialogue)
        {
            Push(_dialogueScene, name, dialogue);
        }
    }
}