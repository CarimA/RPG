using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.Dialogue;
using PhotoVs.Logic.Mechanics.Input;
using PhotoVs.Logic.Mechanics.Input.Components;
using PhotoVs.Logic.NewScenes;
using PhotoVs.Logic.PlayerData;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Logic.Scenes
{
    // horrible port of horrible code but it works
    public class DialogueScene : Scene
    {
        private readonly IAssetLoader _assetLoader;

        private readonly Player _player;
        private readonly SceneMachine _scene;
        private readonly SpriteBatch _spriteBatch;
        private readonly DialogueMarkup _dialogue;

        private readonly string _name;

        public DialogueScene(string name, string dialogue)
        {
            /*_scene = services.Get<SceneMachine>();
            _player = services.Get<Player>();
            _assetLoader = services.Get<IAssetLoader>();
            _spriteBatch = services.Get<SpriteBatch>();*/

            _name = name;
            _dialogue = new DialogueMarkup(_assetLoader.Get<SpriteFont>("ui/fonts/outline_12.fnt"),
                _assetLoader.Get<SpriteFont>("ui/fonts/border_12.fnt"),
                new Vector2(180 + 79, 170 + 109 + 18), //320 - TextWidth - 20, 133),
                dialogue,
                3,
                228);

            IsFinished = false;
        }

        // todo: text log by saving a queue

        public bool IsFinished { get; private set; }

        public bool IsBlocking { get; set; }

        public void Draw(GameTime gameTime)
        {
        }

        public void DrawUI(GameTime gameTime, Matrix uiOrigin)
        {
            var x = 180;
            var y = 170;
            var slice = _assetLoader.Get<Texture2D>("ui/slices/main.png");
            var slice2 = _assetLoader.Get<Texture2D>("ui/slices/main_noborder.png");
            var darkPixel = _assetLoader.Get<Texture2D>("ui/darkblue_pixel.png");
            var pixel = _assetLoader.Get<Texture2D>("ui/blue_pixel.png");

            var portrait = _assetLoader.Get<Texture2D>("portraits/test.png");
            var bold = _assetLoader.Get<SpriteFont>("ui/fonts/bold_outline_12.fnt");

            _spriteBatch.Begin(samplerState: SamplerState.PointWrap, transformMatrix: uiOrigin);

            _spriteBatch.Draw(pixel, new Rectangle(x + 12, y + 108, 294, 58), new Rectangle(0, 0, 1, 1), Color.White);
            _spriteBatch.Draw(darkPixel, new Rectangle(x + 70, y + 108, 236, 16), new Rectangle(0, 0, 1, 1),
                Color.White);
            _spriteBatch.Draw(portrait, new Vector2(x + 12, y + 108), Color.White);
            _spriteBatch.DrawNineSlice(slice, new Rectangle(x + 7, y + 103, 304, 68), new Rectangle(0, 0, 21, 21));
            _spriteBatch.DrawNineSlice(slice2, new Rectangle(x + 7, y + 103, 68, 68), new Rectangle(0, 0, 21, 21));

            _spriteBatch.DrawString(bold, _name, new Vector2(x + 79, y + 109), Color.Yellow);
            _dialogue.Draw(gameTime, _spriteBatch);

            if (_dialogue.IsPaused || _dialogue.IsFinished)
            {
                var next = _assetLoader.Get<Texture2D>("ui/next.png");
                var drift = (float) Math.Sin(gameTime.TotalGameTime.TotalSeconds * 8) * 2 - 1;
                _spriteBatch.Draw(next, new Vector2(x + 291, y + 162 + drift), Color.White);
            }

            _spriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            var input = _player.Components.Get<CInputState>();

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

        public void Enter(params object[] args)
        {
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