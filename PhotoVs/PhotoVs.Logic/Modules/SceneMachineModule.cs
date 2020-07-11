using PhotoVs.Engine.Scripting;
using PhotoVs.Logic.Scenes;
using System;
using PhotoVs.Engine;
using PhotoVs.Logic.NewScenes;
using PhotoVs.Logic.NewScenes.GameScenes;

namespace PhotoVs.Logic.Modules
{
    public class SceneMachineModule : Module
    {
        private readonly Services _services;
        private readonly SceneMachine _sceneMachine;

        public SceneMachineModule(Services services)
        {
            _services = services;
            _sceneMachine = services.Get<SceneMachine>();
        }

        public override void DefineApi(MoonSharpInterpreter interpreter)
        {
            if (interpreter == null)
                throw new ArgumentNullException(nameof(interpreter));

            interpreter.AddFunction("PushScene", (Action<string>)PushScene);

            base.DefineApi(interpreter);
        }

        private void PushScene(string sceneName)
        {
            switch (sceneName)
            {
                case "controller":
                    //_sceneMachine.Push(new ControllerRecommendationScreen(_sceneMachine));
                    _sceneMachine.Push(new WorldScene(_services));
                    _sceneMachine.Push(new TitleScene(_services));
                    //_sceneMachine.Push(new WorldLogicScene(_services));
                    break;

                default:
                    throw new ArgumentException(nameof(sceneName));
            }
        }
    }
}
