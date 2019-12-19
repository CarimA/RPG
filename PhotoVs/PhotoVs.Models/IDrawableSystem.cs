using Microsoft.Xna.Framework;

namespace PhotoVs.Models.ECS
{
    public interface IDrawableSystem : ISystem
    {
        void BeforeDraw(GameTime gameTime);
        void Draw(GameTime gameTime, IGameObjectCollection entities);
        void AfterDraw(GameTime gameTime);
    }
}