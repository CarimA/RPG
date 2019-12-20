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
            sceneMachine.PushDialogueScene(name, dialogue);
        }

        public bool Continue(GameTime gameTime)
        {
            if (_sceneMachine.Peek() is DialogueScene dialogueScene
                && dialogueScene.IsFinished)
            {
                _sceneMachine.Pop();
                return true;
            }

            return false;
        }
    }
}