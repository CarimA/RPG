using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Dialogue;
using PhotoVs.Engine.Graphics;
using PhotoVs.Logic.Input;
using PhotoVs.Models.FSM;
using PhotoVs.Utils;

namespace PhotoVs.Logic.Scenes
{
    // horrible port of horrible code but it works
    public class DialogueScene : IUpdateableScene, IDrawableScene
    {
        private readonly SceneMachine _scene;
        private DialogueMarkup _dialogue;

        private string _name;
        private ShakingBox _shakingBox;

        // todo: text log by saving a queue

        public bool IsFinished { get; private set; }

        public DialogueScene(SceneMachine scene)
        {
            _scene = scene;
        }

        public void Draw(GameTime gameTime)
        {
        }

        public void DrawUI(GameTime gameTime)
        {
            var spriteBatch = _scene.Services.SpriteBatch;
            var assetLoader = _scene.Services.AssetLoader;
            var text = _scene.Services.TextDatabase;

            spriteBatch.Begin();
            spriteBatch.Draw(assetLoader.GetAsset<Texture2D>("portraits/test2.png"), new Vector2(0, 0),
                Color.White);
            spriteBatch.End();

            spriteBatch.Begin(rasterizerState: RasterizerState.CullNone, samplerState: SamplerState.PointClamp);
            _shakingBox.Draw(gameTime);
            spriteBatch.DrawString(text.GetFont(), _name, new Vector2(126 * 4, 94 * 4), Color.White);
            _dialogue.Draw(gameTime, spriteBatch);
            spriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            var input = _scene.Services.Player.Input;

            _shakingBox.Update(gameTime);

            _dialogue.FastForward = input.ActionDown(InputActions.Run);
            _dialogue.Update(gameTime);

            if (_dialogue.IsFinished)
            {
                if (input.ActionPressed(InputActions.Action) || _dialogue.FastForward) IsFinished = true;
            }
            else
            {
                if (_dialogue.IsPaused)
                    if (input.ActionPressed(InputActions.Action))
                        _dialogue.Next();
            }
        }

        public bool IsBlocking { get; set; }

        public void Enter(params object[] args)
        {
            var spriteBatch = _scene.Services.SpriteBatch;
            var assetLoader = _scene.Services.AssetLoader;
            var text = _scene.Services.TextDatabase;

            _name = args[0].ToString();
            var dialogue = args[1].ToString();

            var x = 110 * 4;
            var y = 110 * 4;
            _shakingBox = new ShakingBox(spriteBatch, new List<RectangleF>
            {
                new RectangleF(x, y, 200 * 4, 65 * 4),
                new RectangleF(x + (15 - 3) * 4, y - (20 * 4), 90 * 4, 25 * 4)
            });

            _dialogue = new DialogueMarkup(text.GetFont(),
                new Vector2(113 * 4, 114 * 4), //320 - TextWidth - 20, 133),
                dialogue,
                3,
                200 * 4);

            IsFinished = false;
        }

        public void Exit()
        {
        }

        public void Resume()
        {
        }

        public void Suspend()
        {
        }
    }
}