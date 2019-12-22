using PhotoVs.Engine.FSM.States;
using PhotoVs.Logic.Services;
using PhotoVs.Models.FSM;

namespace PhotoVs.Logic.Scenes
{
    public class SceneMachine : StateMachine<IScene>
    {
        private readonly DialogueScene _dialogueScene;
        private readonly OverworldScene _overworldScene;
        private readonly TextInputScene _textInputScene;

        public ServiceLocator Services { get; }

        public SceneMachine(ServiceLocator services)
        {
            Services = services;
            _overworldScene = new OverworldScene(this);
            _dialogueScene = new DialogueScene(this);
            _textInputScene = new TextInputScene(this);
        }

        public void ChangeToOverworldScene()
        {
            Change(_overworldScene);
        }

        public void PushDialogueScene(string name, string dialogue)
        {
            Push(_dialogueScene, name, dialogue);
        }

        public void PushTextInputScene(string question, string defaultText = "", int limit = 15)
        {
            Push(_textInputScene, question, limit, defaultText);
        }
    }
}