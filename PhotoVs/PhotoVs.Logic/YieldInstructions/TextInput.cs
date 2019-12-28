using Microsoft.Xna.Framework;
using PhotoVs.Logic.Scenes;

namespace PhotoVs.Engine.Scheduler.YieldInstructions
{
    public class TextInput : IYieldInstruction
    {
        private readonly SceneMachine _sceneMachine;
        public string Text { get; set; }

        public TextInput(SceneMachine sceneMachine, string question, string defaultText = "", int limit = 15)
        {
            _sceneMachine = sceneMachine;

            if (sceneMachine.Peek() is OverworldScene overworld)
                overworld.PushTextInputScene(question, defaultText, limit);

            // todo: throw when not a matching scene
        }

        public bool Continue(GameTime gameTime)
        {
            if (_sceneMachine.Peek() is TextInputScene textInputScene
                && textInputScene.IsFinished)
            {
                Text = textInputScene.Text;
                _sceneMachine.Pop();
                return true;
            }

            return false;
        }
    }
}