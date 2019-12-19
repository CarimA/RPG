using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine;
using PhotoVs.Engine.FSM.States;
using PhotoVs.Logic.Camera;
using PhotoVs.Logic.PlayerData;
using PhotoVs.Logic.Services;
using PhotoVs.Models.Assets;
using PhotoVs.Models.FSM;

namespace PhotoVs.Logic.Scenes
{
    public class SceneMachine : StateMachine<IScene>
    {
        private readonly OverworldScene _overworldScene;
        private readonly DialogueScene _dialogueScene;

        public SceneMachine(ServiceLocator services)
        {
            Services = services;
            _overworldScene = new OverworldScene(this);
            _dialogueScene = new DialogueScene(this);
        }

        public ServiceLocator Services { get; private set; }

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