using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Dialogue;
using PhotoVs.Logic.PlayerData;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.FSM.Scenes;
using PhotoVs.Logic.Mechanics.Input;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Logic.Scenes
{
    // horrible port of horrible code but it works
    public class DialogueScene : IUpdateableScene, IDrawableScene
    {
        private readonly IAssetLoader _assetLoader;

        private readonly Player _player;
        private readonly SceneMachine _scene;
        private readonly SpriteBatch _spriteBatch;
        private DialogueMarkup _dialogue;

        private string _name;

        // todo: text log by saving a queue

        public bool IsFinished { get; private set; }

        public DialogueScene(SceneMachine scene)
        {
            _scene = scene;
            _player = _scene.Services.Get<Player>();
            _assetLoader = _scene.Services.Get<IAssetLoader>();
            _spriteBatch = _scene.Services.Get<SpriteBatch>();
        }

        public void Draw(GameTime gameTime)
        {
        }

        public void DrawUI(GameTime gameTime, Matrix uiOrigin)
        {
            var x = 180;
            var y = 170;
            var slice = _assetLoader.GetAsset<Texture2D>("ui/slices/main.png");
            var slice2 = _assetLoader.GetAsset<Texture2D>("ui/slices/main_noborder.png");
            var darkPixel = _assetLoader.GetAsset<Texture2D>("ui/darkblue_pixel.png");
            var pixel = _assetLoader.GetAsset<Texture2D>("ui/blue_pixel.png");

            var portrait = _assetLoader.GetAsset<Texture2D>("portraits/test.png");
            var bold = _assetLoader.GetAsset<SpriteFont>("ui/fonts/bold_outline_12.fnt");

            _spriteBatch.Begin(samplerState: SamplerState.PointWrap, transformMatrix: uiOrigin);

            _spriteBatch.Draw(pixel, new Rectangle(x + 12, y + 108, 294, 58), new Rectangle(0, 0, 1, 1), Color.White);
            _spriteBatch.Draw(darkPixel, new Rectangle(x + 70, y + 108, 236, 16), new Rectangle(0, 0, 1, 1), Color.White);
            _spriteBatch.Draw(portrait, new Vector2(x + 12, y + 108), Color.White);
            _spriteBatch.DrawNineSlice(slice, new Rectangle(x + 7, y + 103, 304, 68), new Rectangle(0, 0, 21, 21));
            _spriteBatch.DrawNineSlice(slice2, new Rectangle(x + 7, y + 103, 68, 68), new Rectangle(0, 0, 21, 21));

            _spriteBatch.DrawString(bold, _name, new Vector2(x + 79, y + 109), Color.Yellow);
            _dialogue.Draw(gameTime, _spriteBatch);

            if (_dialogue.IsPaused || _dialogue.IsFinished)
            {
                var next = _assetLoader.GetAsset<Texture2D>("ui/next.png");
                var drift = ((float) System.Math.Sin(gameTime.TotalGameTime.TotalSeconds * 8) * 2) - 1;
                _spriteBatch.Draw(next, new Vector2(x + 291, y + 162 + drift), Color.White);
            }

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