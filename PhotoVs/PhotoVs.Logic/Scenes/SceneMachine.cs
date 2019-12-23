using PhotoVs.Engine.FSM.States;
using PhotoVs.Logic.Services;
using PhotoVs.Models.FSM;

namespace PhotoVs.Logic.Scenes
{
    public class SceneMachine : StateMachine<IScene>
    {
        public ServiceLocator Services { get; }

        internal DialogueScene DialogueScene { get; }
        internal TextInputScene TextInputScene { get; }
        internal OverworldScene OverworldScene { get; }

        public SceneMachine(ServiceLocator services)
        {
            Services = services;
            OverworldScene = new OverworldScene(this);
            DialogueScene = new DialogueScene(this);
            TextInputScene = new TextInputScene(this);
        }

        // todo: move to individual states to control scene flow and
        // ensure that you can't call incorrect scenes from wrong places
        public void ChangeToOverworldScene()
        {
            Change(OverworldScene);
        }
    }
}