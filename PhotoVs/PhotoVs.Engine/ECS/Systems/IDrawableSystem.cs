using Microsoft.Xna.Framework;
using PhotoVs.Engine.ECS.GameObjects;

namespace PhotoVs.Engine.ECS.Systems
{
    public interface IDrawableSystem : ISystem
    {
        void BeforeDraw(GameTime gameTime);
        void Draw(GameTime gameTime, IGameObjectCollection entities);
        void AfterDraw(GameTime gameTime);
        void DrawUI(GameTime gameTime, IGameObjectCollection gameObjectCollection, Matrix uiOrigin);
    }
}