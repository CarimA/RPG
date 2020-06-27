using System;
using PhotoVs.Engine.Scripting;
using PhotoVs.Logic.Scenes;

namespace PhotoVs.Logic.Modules
{
    public class DialogueModule : Module
    {
        private SceneMachine _sceneMachine;

        public DialogueModule(SceneMachine sceneMachine)
        {
            _sceneMachine = sceneMachine;
        }

        public override void DefineApi(MoonSharpInterpreter interpreter)
        {
            if (interpreter == null) throw new ArgumentNullException(nameof(interpreter));

            interpreter.AddFunction("_say", (Func<string, string, bool>)DialogueState);
            interpreter.RunScript($@"
                function say(n, d)
                    while _say(n, d) do
                        coroutine.yield()
                    end
                end");

            base.DefineApi(interpreter);
        }

        private bool DialogueState(string name, string dialogue)
        {
            if (_sceneMachine.Peek() is OverworldScene overworld)
            {
                overworld.PushDialogue(name, dialogue);
                return true;
            }

            if (_sceneMachine.Peek() is DialogueScene dialogueScene && dialogueScene.IsFinished)
            {
                _sceneMachine.Pop();
                return false;
            }

            return true;
        }
    }
}
