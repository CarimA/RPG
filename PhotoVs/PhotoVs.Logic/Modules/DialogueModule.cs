using System;
using MoonSharp.Interpreter;
using PhotoVs.Engine.Scripting;
using PhotoVs.Logic.NewScenes;
using PhotoVs.Logic.Scenes;

namespace PhotoVs.Logic.Modules
{
    public class DialogueModule
    {
        private readonly SceneMachine _sceneMachine;

        public DialogueModule(IInterpreter<Closure> interpreter, SceneMachine sceneMachine)
        {
            _sceneMachine = sceneMachine;

            interpreter.AddFunction("_Say", (Func<string, string, bool>) DialogueState);
        }

        private bool DialogueState(string name, string dialogue)
        {
            if (_sceneMachine.Peek() is DialogueScene dialogueScene && dialogueScene.IsFinished)
            {
                _sceneMachine.Pop();
                return false;
            }

            //overworld.PushDialogue(name, dialogue);
            return true;
        }
    }
}