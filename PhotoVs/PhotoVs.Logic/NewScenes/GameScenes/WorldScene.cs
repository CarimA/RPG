using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.Core;
using PhotoVs.Engine.ECS;
using PhotoVs.Engine.Graphics;
using PhotoVs.Logic.Mechanics.World;
using PhotoVs.Logic.Mechanics.World.Systems;

namespace PhotoVs.Logic.NewScenes.GameScenes
{
    public class WorldScene : Scene
    {
        public WorldScene(IAssetLoader assetLoader, IRenderer renderer, IOverworld overworld, SpriteBatch spriteBatch,
            IGameState gameState,
            ISignal signal, GraphicsDevice graphicsDevice, ICanvasSize canvasSize)
            : base(new SystemList
            {
                new SRenderOverworld(assetLoader, renderer, overworld, spriteBatch, gameState, signal, graphicsDevice,
                    canvasSize)
            })
        {
        }

        public override bool IsBlocking => true;
    }
}