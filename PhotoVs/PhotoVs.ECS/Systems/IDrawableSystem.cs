using Microsoft.Xna.Framework;
using PhotoVs.ECS.Entities;

namespace PhotoVs.ECS.Systems
{
    public interface IDrawableSystem : ISystem
    {
        void BeforeDraw(GameTime gameTime);
        void Draw(GameTime gameTime, EntityCollection entities);
        void AfterDraw(GameTime gameTime);
    }
}