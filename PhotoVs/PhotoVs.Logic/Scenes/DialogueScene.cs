using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private SceneMachine _scene;
        private SpriteBatch _spriteBatch => _scene.SpriteBatch;
        private IAssetLoader _assetLoader => _scene.AssetLoader;
        private GameInput _input => _scene.Player.Input.Input;

        private const float TextScrollSpeed = 22f;

        private string _name;

        private DialogueMarkup _dialogue;
        private ShakingBox _shakingBox;
        private bool _isFinished;

        public DialogueScene(SceneMachine scene)
        {
            _scene = scene;
        }

        public void Update(GameTime gameTime)
        {
            _shakingBox.Update(gameTime);

            _dialogue.SetFastForward(_input.ActionDown(InputActions.Run));
            _dialogue.Update(gameTime);

            if (_dialogue.IsFinished())
            {
                if (_input.ActionPressed(InputActions.Action))
                {
                    _isFinished = true;
                }
            }
            else
            {
                if (_dialogue.IsPaused())
                {
                    if (_input.ActionPressed(InputActions.Action))
                    {
                        _dialogue.Next();
                    }
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(_assetLoader.GetAsset<Texture2D>("portraits/test3.png"), new Vector2(0, 180 - 165), Color.White);
            _spriteBatch.End();

            _spriteBatch.Begin(rasterizerState: RasterizerState.CullNone, samplerState: SamplerState.PointClamp);
            _shakingBox.Draw(gameTime);
            var font = _assetLoader.GetAsset<BitmapFont>("fonts/body.fnt");
            _spriteBatch.DrawString(font, _name, new Vector2(126, 94), Color.White);
            _dialogue.Draw(gameTime, _spriteBatch);
            _spriteBatch.End();
        }

        public bool IsBlocking { get; set; }

        public void Enter(params object[] args)
        {
            _name = args[0].ToString();
            var text = args[1].ToString();

            var x = 110;
            var y = 110;
            _shakingBox = new ShakingBox(_spriteBatch, new List<RectangleF>()
            {
                new RectangleF(x, y, 200, 65),
                new RectangleF(x + 15 - 3, y - 20, 90, 25)
            });

            _dialogue = new DialogueMarkup(_assetLoader.GetAsset<BitmapFont>("fonts/body.fnt"),
                new Vector2(113, 114), //320 - TextWidth - 20, 133),
                text,
                3,
                200);

            _isFinished = false;
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

        public bool IsFinished()
        {
            return _isFinished;
        }
    }
}
