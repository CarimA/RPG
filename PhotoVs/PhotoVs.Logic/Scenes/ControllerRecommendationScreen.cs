using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.ECS.GameObjects;
using PhotoVs.Engine.ECS.Systems;
using PhotoVs.Logic.PlayerData;
using PhotoVs.Logic.Text;
using PhotoVs.Models.Assets;
using PhotoVs.Models.ECS;
using PhotoVs.Models.FSM;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Logic.Scenes
{
    public class ControllerRecommendationScreen : IUpdateableScene, IDrawableScene, ISystemScene
    {
        private readonly IAssetLoader _assetLoader;
        private readonly Player _player;
        private readonly SceneMachine _scene;
        private readonly SpriteBatch _spriteBatch;
        private readonly TextDatabase _textDatabase;
        private float _continueTime;
        private string _copyrightNotice;
        private SpriteFont _font;
        private Texture2D _gamepadIcon;
        private bool _isChanging;
        private string _playWithAGamepad;

        public ControllerRecommendationScreen(SceneMachine scene)
        {
            _scene = scene;
            _player = _scene.Services.Get<Player>();
            _textDatabase = _scene.Services.Get<TextDatabase>();
            _assetLoader = _scene.Services.Get<IAssetLoader>();
            _spriteBatch = _scene.Services.Get<SpriteBatch>();
            Entities = new GameObjectCollection();
            Systems = new SystemCollection();
        }

        public void Draw(GameTime gameTime)
        {
        }

        public void DrawUI(GameTime gameTime)
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp);

            _spriteBatch.DrawStringCenterTopAligned(_font, _playWithAGamepad, new Vector2(160, 10), Color.White);
            _spriteBatch.DrawStringCenterTopAligned(_font, _copyrightNotice, new Vector2(160, 146),
                Color.White);

            _spriteBatch.Draw(_gamepadIcon, new Vector2(124, 68), new Rectangle(1, 1, 73, 45), Color.White);

            _spriteBatch.End();
        }

        public IGameObjectCollection Entities { get; }
        public ISystemCollection Systems { get; }
        public bool IsBlocking { get; set; }

        public void Enter(params object[] args)
        {
            _continueTime = 16f;
            _isChanging = false;

            var font = _textDatabase.GetFont();

            _gamepadIcon = _assetLoader.GetAsset<Texture2D>("ui/gamepad.png");
            _font = font;

            _playWithAGamepad = _font.WrapText(_textDatabase.GetText("CR_PlayWithAGamepad"), 320  - 40);
            _copyrightNotice = _font.WrapText(_textDatabase.GetText("CR_CopyrightNotice"), 320  - 80);
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
            if (_continueTime <= 0f || _player.Input.AnyActionDown())
            {
                _scene.ChangeToOverworldScene();
                _isChanging = true;
            }
        }
    }
}