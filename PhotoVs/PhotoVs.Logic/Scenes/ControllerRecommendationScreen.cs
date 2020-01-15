using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.ECS.GameObjects;
using PhotoVs.Engine.ECS.Systems;
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
        private SpriteFont _font;
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
        }

        public void DrawUI(GameTime gameTime)
        {
            var spriteBatch = _scene.Services.SpriteBatch;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp);

            spriteBatch.DrawStringCenterTopAligned(_font, _playWithAGamepad, new Vector2(320, 10), Color.White);
            spriteBatch.DrawStringCenterTopAligned(_font, _copyrightNotice, new Vector2(320, (180 * 2) - 80),
                Color.White);

            spriteBatch.Draw(_gamepadIcon, new Vector2(96, 82), Color.White);

            spriteBatch.End();
        }

        public IGameObjectCollection Entities { get; }
        public ISystemCollection Systems { get; }
        public bool IsBlocking { get; set; }

        public void Enter(params object[] args)
        {
            _continueTime = 16f;
            _isChanging = false;

            var textDatabase = _scene.Services.TextDatabase;
            var assetLoader = _scene.Services.AssetLoader;
            var text = _scene.Services.TextDatabase;
            var font = text.GetFont();

            _gamepadIcon = assetLoader.GetAsset<Texture2D>("interfaces/gamepad.png");
            _font = font;

            _playWithAGamepad = _font.WrapText(textDatabase.GetText("CR_PlayWithAGamepad"), (320 * 2) - 40);
            _copyrightNotice = _font.WrapText(textDatabase.GetText("CR_CopyrightNotice"), (320 * 2) - 80);
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