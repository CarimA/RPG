﻿using System;
using PhotoVs.Engine.Scripting;
using PhotoVs.Logic.Scenes;

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
            interpreter.AddFunction("push_scene", (Action<string>)PushScene);

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