using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Dialogue;
using PhotoVs.Engine.Graphics;
using PhotoVs.Logic.Input;
using PhotoVs.Logic.PlayerData;
using PhotoVs.Models.Assets;
using PhotoVs.Models.FSM;
using PhotoVs.Models.Text;
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

        private readonly Player _player;
        private readonly ITextDatabase _textDatabase;
        private readonly IAssetLoader _assetLoader;
        private readonly SpriteBatch _spriteBatch;

        // todo: text log by saving a queue

        public bool IsFinished { get; private set; }

        public DialogueScene(SceneMachine scene)
        {
            _scene = scene;
            _player = _scene.Services.Get<Player>();
            _textDatabase = _scene.Services.Get<ITextDatabase>();
            _assetLoader = _scene.Services.Get<IAssetLoader>();
            _spriteBatch = _scene.Services.Get<SpriteBatch>();
        }

        public void Draw(GameTime gameTime)
        {
        }

        public void DrawUI(GameTime gameTime)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(_assetLoader.GetAsset<Texture2D>("portraits/test2.png"), new Vector2(0, 0),
                Color.White);
            _spriteBatch.End();

            _spriteBatch.Begin(rasterizerState: RasterizerState.CullNone, samplerState: SamplerState.PointClamp);
            _shakingBox.Draw(gameTime);
            _spriteBatch.DrawString(_textDatabase.GetFont(), _name, new Vector2(126 * 2, 94 * 2), Color.White);
            _dialogue.Draw(gameTime, _spriteBatch);
            _spriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            var input = _player.Input;

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
            _name = args[0].ToString();
            var dialogue = args[1].ToString();

            var x = 110 * 2;
            var y = 110 * 2;
            _shakingBox = new ShakingBox(_spriteBatch, new List<RectangleF>
            {
                new RectangleF(x, y, 200 * 2, 65 * 2),
                new RectangleF(x + (15 - 3) * 2, y - (20 * 2), 90 * 2, 25 * 2)
            });

            _dialogue = new DialogueMarkup(_textDatabase.GetFont(),
                new Vector2(113 * 2, 114 * 2), //320 - TextWidth - 20, 133),
                dialogue,
                3,
                200 * 2);

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