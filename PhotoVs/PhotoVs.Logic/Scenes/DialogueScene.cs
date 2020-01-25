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
        private readonly IAssetLoader _assetLoader;

        private readonly Player _player;
        private readonly SceneMachine _scene;
        private readonly SpriteBatch _spriteBatch;
        private readonly ITextDatabase _textDatabase;
        private DialogueMarkup _dialogue;

        private string _name;

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
            var dialogue = _assetLoader.GetAsset<Texture2D>("ui/dialogue.png");
            var portrait = _assetLoader.GetAsset<Texture2D>("portraits/test.png");
            var bold = _assetLoader.GetAsset<SpriteFont>("ui/fonts/bold_outline_12.fnt");

            _spriteBatch.Begin(samplerState: SamplerState.PointWrap);
            _spriteBatch.Draw(dialogue, new Vector2(12, 108), new Rectangle(304, 0, 58, 58), Color.White);
            _spriteBatch.Draw(portrait, new Vector2(12, 108), Color.White);
            _spriteBatch.Draw(dialogue, new Vector2(7, 103), new Rectangle(0, 0, 304, 68), Color.White);
            _spriteBatch.DrawString(bold, _name, new Vector2(79, 109), Color.Yellow);
            _dialogue.Draw(gameTime, _spriteBatch);
            _spriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            var input = _player.Input;


            _dialogue.FastForward = input.ActionDown(InputActions.Run);
            _dialogue.Update(gameTime);

            if (_dialogue.IsFinished)
            {
                if (input.ActionPressed(InputActions.Action) || _dialogue.FastForward)
                    IsFinished = true;
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

            _dialogue = new DialogueMarkup(_assetLoader.GetAsset<SpriteFont>("ui/fonts/outline_12.fnt"), 
                _assetLoader.GetAsset<SpriteFont>("ui/fonts/border_12.fnt"),
                new Vector2(79, 126), //320 - TextWidth - 20, 133),
                dialogue,
                3,
                228 );

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