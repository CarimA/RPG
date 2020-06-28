using Microsoft.Xna.Framework;

namespace PhotoVs.Engine.ECS.Systems
{
    public interface IDrawableSystem : ISystem
    {
        void BeforeDraw(GameTime gameTime);
        void Draw(GameTime gameTime, GameObjectList entities);
        void AfterDraw(GameTime gameTime);
        void DrawUI(GameTime gameTime, GameObjectList gameObjectCollection, Matrix uiOrigin);
    }
}