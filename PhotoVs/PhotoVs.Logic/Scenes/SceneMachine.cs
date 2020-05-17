using Microsoft.Xna.Framework;
using PhotoVs.Engine;
using PhotoVs.Engine.FSM.Scenes;
using PhotoVs.Engine.FSM.States;
using PhotoVs.Logic.Scenes.Transitions;
using PhotoVs.Engine.ECS;
using PhotoVs.Engine.ECS.GameObjects;
using PhotoVs.Engine.ECS.Systems;
using PhotoVs.Engine.FSM;

namespace PhotoVs.Logic.Scenes
{
    public class SceneMachine : StateMachine<IScene>
    {
        private ITransition _activeTransition;
        private object[] _nextArgs;
        private IScene _nextState;

        public Services Services { get; }
        public ISceneManager SceneManager { get; }

        public ControllerRecommendationScreen ControllerRecommendationScreen { get; }
        internal DialogueScene DialogueScene { get; }
        internal TextInputScene TextInputScene { get; }
        internal OverworldScene OverworldScene { get; }

        public SceneMachine(Services services)
        {
            Services = services;
            ControllerRecommendationScreen = new ControllerRecommendationScreen(this);
            OverworldScene = new OverworldScene(this);
            DialogueScene = new DialogueScene(this);
            TextInputScene = new TextInputScene(this);

            SceneManager = new SceneManager(this,
                Services.Get<ISystemCollection<ISystem>>(),
                Services.Get<IGameObjectCollection>());
        }

        // todo: move to individual states to control scene flow and
        // ensure that you can't call incorrect scenes from wrong places
        public void ChangeToOverworldScene()
        {
            Change(new FadeTransition(Services, Color.Black), OverworldScene);
        }

        public void Change(ITransition transition, IScene state, params object[] args)
        {
            _activeTransition = transition;
            _nextState = state;
            _nextArgs = args;
        }

        public void Update(GameTime gameTime)
        {
            SceneManager.Update(gameTime);

            if (_activeTransition == null)
                return;
            _activeTransition.Update(gameTime);

            if (_activeTransition.ShouldSwitch())
                base.Change(_nextState, _nextArgs);

            if (_activeTransition.IsFinished)
            {
                _activeTransition = null;
                _nextState = null;
                _nextArgs = null;
            }
        }

        public void Draw(GameTime gameTime)
        {
            SceneManager.Draw(gameTime);
        }

        public void DrawUI(GameTime gameTime, Matrix uiOrigin)
        {
            SceneManager.DrawUI(gameTime, uiOrigin);
            _activeTransition?.Draw(gameTime);
        }
    }
}