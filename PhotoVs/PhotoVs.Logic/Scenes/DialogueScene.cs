using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Dialogue;
using PhotoVs.Engine.Graphics;
using PhotoVs.Engine.Graphics.BitmapFonts;
using PhotoVs.Logic.Input;
using PhotoVs.Models.Assets;
using PhotoVs.Models.FSM;
using PhotoVs.Utils;

namespace PhotoVs.Logic.Scenes
{
    // horrible port of horrible code but it works
    public class DialogueScene : IUpdateableScene, IDrawableScene
    {
        private DialogueMarkup _dialogue;

        private string _name;
        private readonly SceneMachine _scene;
        private ShakingBox _shakingBox;
        private SpriteBatch _spriteBatch => _scene.Services.SpriteBatch;
        private IAssetLoader _assetLoader => _scene.Services.AssetLoader;
        private GameInput _input => _scene.Services.Player.Input;

        public bool IsFinished { get; private set; }

        public DialogueScene(SceneMachine scene)
        {
            _scene = scene;
        }

        public void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(_assetLoader.GetAsset<Texture2D>("portraits/test3.png"), new Vector2(0, 180 - 165),
                Color.White);
            _spriteBatch.End();

            _spriteBatch.Begin(rasterizerState: RasterizerState.CullNone, samplerState: SamplerState.PointClamp);
            _shakingBox.Draw(gameTime);
            var font = _assetLoader.GetAsset<BitmapFont>("fonts/body.fnt");
            _spriteBatch.DrawString(font, _name, new Vector2(126, 94), Color.White);
            _dialogue.Draw(gameTime, _spriteBatch);
            _spriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            _shakingBox.Update(gameTime);

            _dialogue.FastForward = _input.ActionDown(InputActions.Run);
            _dialogue.Update(gameTime);

            if (_dialogue.IsFinished())
            {
                if (_input.ActionPressed(InputActions.Action)) IsFinished = true;
            }
            else
            {
                if (_dialogue.IsPaused())
                    if (_input.ActionPressed(InputActions.Action))
                        _dialogue.Next();
            }
        }

        public bool IsBlocking { get; set; }

        public void Enter(params object[] args)
        {
            _name = args[0].ToString();
            var text = args[1].ToString();

            var x = 110;
            var y = 110;
            _shakingBox = new ShakingBox(_spriteBatch, new List<RectangleF>
            {
                new RectangleF(x, y, 200, 65),
                new RectangleF(x + 15 - 3, y - 20, 90, 25)
            });

            _dialogue = new DialogueMarkup(_assetLoader.GetAsset<BitmapFont>("fonts/body.fnt"),
                new Vector2(113, 114), //320 - TextWidth - 20, 133),
                text,
                3,
                200);

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