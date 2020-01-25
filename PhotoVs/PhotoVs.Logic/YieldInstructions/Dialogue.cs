using Microsoft.Xna.Framework;
using PhotoVs.Logic.Scenes;

namespace PhotoVs.Engine.Scheduler.YieldInstructions
{
    public class Dialogue : IYieldInstruction
    {
        private readonly SceneMachine _sceneMachine;

        public Dialogue(SceneMachine sceneMachine, string name, string dialogue)
        {
            _sceneMachine = sceneMachine;

            if (sceneMachine.Peek() is OverworldScene overworld)
                overworld.PushDialogue(name, dialogue);

            // todo: throw when not a matching scene
        }

        public bool Continue(GameTime gameTime)
        {
            if (_sceneMachine.Peek() is DialogueScene dialogueScene
                && dialogueScene.IsFinished)
            {
                _sceneMachine.Pop();
                return true;
            }

            if (!(_sceneMachine.Peek() is DialogueScene))
            {
                return true;
            }

            return false;
        }
    }
}