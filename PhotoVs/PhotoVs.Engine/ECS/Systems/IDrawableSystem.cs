using Microsoft.Xna.Framework;

namespace PhotoVs.Engine.ECS
{
    public interface IDrawableSystem : ISystem
    {
        void BeforeDraw(GameTime gameTime);
        void Draw(GameTime gameTime, IGameObjectCollection entities);
        void AfterDraw(GameTime gameTime);
        void DrawUI(GameTime gameTime, IGameObjectCollection gameObjectCollection, Matrix uiOrigin);
    }
}