using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.ECS;
using PhotoVs.Engine.Graphics;
using PhotoVs.Logic.Mechanics.Input;
using PhotoVs.Logic.Mechanics.Input.Components;
using PhotoVs.Logic.NewScenes.SceneComponents;
using PhotoVs.Utils;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Logic.NewScenes.GameScenes
{
    public class TitleScene : Scene
    {
        private readonly Renderer _renderer;
        private readonly SpriteBatch _spriteBatch;
        private readonly Texture2D _ui;
        private readonly Texture2D _filmdust;
        private readonly HorizontalMenu _menu;
        private readonly int menuHeight = 25;

        private readonly AnimatedRectangle _button;

        public override bool IsBlocking => false;

        public TitleScene(Services services) : base(new SystemList())
        {
            var assetLoader = services.Get<IAssetLoader>();
            _renderer = services.Get<Renderer>();
            _spriteBatch = services.Get<SpriteBatch>();
            _ui = assetLoader.Get<Texture2D>("ui/title.png");

            var padding = 60;

            _menu = new HorizontalMenu(
                new Rectangle(
                    padding,
                    _renderer.VirtualHeight - padding - menuHeight,
                    _renderer.VirtualWidth - (padding * 2),
                    menuHeight),
                assetLoader.Get<SpriteFont>("ui/fonts/plain_12.fnt"),
                assetLoader.Get<SpriteFont>("ui/fonts/border_12.fnt"));
            _menu.TextColor = new Color(35, 25, 21); //new Color(207, 199, 186);
            _menu.HighlightColor = new Color(35, 25, 21); //Color.Magenta; // new Color(35, 25, 21);
            _menu.OutlineColor = new Color(207, 199, 186);
            _menu.HighlightOutlineColor = new Color(207, 199, 186);
            _menu.Add(new MenuItem("New Game", Dummy));
            _menu.Add(new MenuItem("Load Game", Dummy));
            _menu.Add(new MenuItem("Settings", Dummy));
            _menu.Add(new MenuItem("Mods", Dummy));
            _menu.Add(new MenuItem("Credits", Dummy));
            _menu.Add(new MenuItem("Quit", Dummy));

            _button = new AnimatedRectangle(_menu.GetHighlightedItemPosition(), TimeSpan.FromSeconds(0.135f), Easings.Functions.EaseOutBack);
        }

        private void Dummy()
        {

        }

        public override void Update(GameTime gameTime)
        {
            _button.Update(gameTime);

            base.Update(gameTime);
        }

        public override void ProcessInput(GameTime gameTime, CInputState inputState)
        {
            var squish = 3;
            if (inputState.ActionPressed(InputActions.Left))
            {
                _menu.Previous();
                _button.SetTargetRectangle(_menu.GetHighlightedItemPosition());
            }

            if (inputState.ActionPressed(InputActions.Right))
            {
                _menu.Next();
                _button.SetTargetRectangle(_menu.GetHighlightedItemPosition());
            }

            if (inputState.ActionPressed(InputActions.Action))
                _menu.Click();

            base.ProcessInput(gameTime, inputState);
        }

        public override void Draw(GameTime gameTime, Matrix uiOrigin)
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointWrap);
            _spriteBatch.Draw(_ui,
                new Vector2(_renderer.GameWidth / 2 - 420, _renderer.GameHeight / 2 - 200),
                new Rectangle(0, 400, 840, 400),
                Color.White);

            //_spriteBatch.DrawNineSlice(_ui, new Rectangle(2, 2, _renderer.GameWidth - 4, _renderer.GameHeight - 4), new Rectangle(840, 0, 12, 12));

            _spriteBatch.End();

            _spriteBatch.Begin(samplerState: SamplerState.PointWrap, transformMatrix: uiOrigin);

            _spriteBatch.DrawNineSlice(_ui, _button.Current, new Rectangle(840, 0, 12, 12));
            _menu.Draw(_spriteBatch);

            var offset = (int)(Math.Sin(gameTime.GetTotalSeconds() * 5) * 3);

            _spriteBatch.Draw(_ui, new Vector2(_button.Current.Right + 4 + offset, _button.Current.Top + (_button.Current.Height / 2) - 5), new Rectangle(857, 0, 6, 10), Color.White);
            _spriteBatch.Draw(_ui, new Vector2(_button.Current.Left - 10 - offset, _button.Current.Top + (_button.Current.Height / 2) - 5), new Rectangle(852, 0, 6, 10) , Color.White);

            _spriteBatch.End();

            base.Draw(gameTime, uiOrigin);
        }
    }
}
