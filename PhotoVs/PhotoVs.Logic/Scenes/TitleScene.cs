using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.FSM.Scenes;
using PhotoVs.Logic.Text;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Logic.Scenes
{
    public class TitleScene : IUpdateableScene, IDrawableScene
	{
		private readonly SceneMachine _sceneMachine;
		
		private readonly IAssetLoader _assetLoader;
		private readonly SpriteBatch _spriteBatch;
		private readonly TextDatabase _textDatabase;

		public bool IsBlocking { get; set; } = false;

		public TitleScene(SceneMachine sceneMachine)
		{
			_sceneMachine = sceneMachine;
				
			_assetLoader = sceneMachine.Services.Get<IAssetLoader>();
			_spriteBatch = sceneMachine.Services.Get<SpriteBatch>();
			_textDatabase = sceneMachine.Services.Get<TextDatabase>();
		}

		public void Update(GameTime gameTime)
		{
			
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

		public void Draw(GameTime gameTime)
		{
		}

		public void DrawUI(GameTime gameTime, Matrix uiOrigin)
		{
			var font = _textDatabase.GetFont();

			_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, transformMatrix: uiOrigin);

      _spriteBatch.DrawStringCenterTopAligned(font, "Title Screen", new Vector2(160, 10), Color.White);
            
      _spriteBatch.End();
			
		}
	}
}