using Microsoft.Xna.Framework;
using PhotoVs.Engine.Events.Coroutines.Instructions;
using PhotoVs.Logic.Scenes;

namespace PhotoVs.Logic.Events.Instructions
{
    public class Dialogue : IYield
    {
        private readonly SceneMachine _sceneMachine;

        public Dialogue(SceneMachine sceneMachine, string name, string dialogue)
        {
            _sceneMachine = sceneMachine;

            if (sceneMachine.Peek() is OverworldScene overworld)
                overworld.PushDialogue(name, dialogue);

            // todo: throw when not a matching scene
        }

        public bool CanContinue(GameTime gameTime)
        {
            if (!(_sceneMachine.Peek() is DialogueScene dialogueScene) || !dialogueScene.IsFinished)
                return !(_sceneMachine.Peek() is DialogueScene);
            _sceneMachine.Pop();
            return true;

        }
    }
}
