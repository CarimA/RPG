using PhotoVs.Engine.FSM.States;
using PhotoVs.Logic.Services;
using PhotoVs.Models.FSM;

namespace PhotoVs.Logic.Scenes
{
    public class SceneMachine : StateMachine<IScene>
    {
        private readonly DialogueScene _dialogueScene;
        private readonly OverworldScene _overworldScene;

        public ServiceLocator Services { get; }

        public SceneMachine(ServiceLocator services)
        {
            Services = services;
            _overworldScene = new OverworldScene(this);
            _dialogueScene = new DialogueScene(this);
        }

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