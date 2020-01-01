using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.ECS.GameObjects;
using PhotoVs.Engine.ECS.Systems;
using PhotoVs.Engine.Graphics.BitmapFonts;
using PhotoVs.Models.ECS;
using PhotoVs.Models.FSM;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Logic.Scenes
{
    internal class ControllerRecommendationScreen : IUpdateableScene, IDrawableScene, ISystemScene
    {
        private readonly SceneMachine _scene;
        private float _continueTime;
        private string _copyrightNotice;
        private BitmapFont _font;
        private Texture2D _gamepadIcon;
        private bool _isChanging;
        private string _playWithAGamepad;

        public ControllerRecommendationScreen(SceneMachine scene)
        {
            _scene = scene;
            Entities = new GameObjectCollection();
            Systems = new SystemCollection();
        }

        public void Draw(GameTime gameTime)
        {
            var spriteBatch = _scene.Services.SpriteBatch;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp);

            spriteBatch.DrawStringCenterTopAligned(_font, _playWithAGamepad, new Vector2(320 / 2, 10), Color.White);
            spriteBatch.DrawStringCenterTopAligned(_font, _copyrightNotice, new Vector2(320 / 2, 180 - 20),
                Color.White);

            spriteBatch.Draw(_gamepadIcon, new Vector2(96, 82), Color.White);

            spriteBatch.End();
        }

        public IGameObjectCollection Entities { get; }
        public ISystemCollection Systems { get; }
        public bool IsBlocking { get; set; }

        public void Enter(params object[] args)
        {
            _continueTime = 6f;
            _isChanging = false;

            var textDatabase = _scene.Services.TextDatabase;
            var assetLoader = _scene.Services.AssetLoader;

            _gamepadIcon = assetLoader.GetAsset<Texture2D>("interfaces/gamepad.png");
            _font = assetLoader.GetAsset<BitmapFont>("fonts/body.fnt");

            _playWithAGamepad = _font.WrapText(textDatabase.GetText("CR_PlayWithAGamepad"), 280);
            _copyrightNotice = _font.WrapText(textDatabase.GetText("CR_CopyrightNotice"), 300);
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

        public void Update(GameTime gameTime)
        {
            if (_isChanging)
                return;

            _continueTime -= gameTime.GetElapsedSeconds();
            if (_continueTime <= 0f || _scene.Services.Player.Input.AnyActionDown())
            {
                _scene.ChangeToOverworldScene();
                _isChanging = true;
            }
        }
    }
}