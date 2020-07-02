using PhotoVs.Engine.Scripting;
using PhotoVs.Logic.Scenes;
using System;

namespace PhotoVs.Logic.Modules
{
    public class SceneMachineModule : Module
    {
        private readonly SceneMachine _sceneMachine;

        public SceneMachineModule(SceneMachine sceneMachine)
        {
            _sceneMachine = sceneMachine;
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
                    _sceneMachine.Push(_sceneMachine.ControllerRecommendationScreen);
                    break;

                default:
                    throw new ArgumentException(nameof(sceneName));
            }
        }
    }
}
