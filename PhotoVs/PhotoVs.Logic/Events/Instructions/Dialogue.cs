using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
