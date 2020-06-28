using Microsoft.Xna.Framework;

namespace PhotoVs.Engine.ECS.Systems
{
    public interface IUpdateableSystem : ISystem
    {
        void BeforeUpdate(GameTime gameTime);
        void Update(GameTime gameTime, GameObjectList entities);
        void AfterUpdate(GameTime gameTime);
    }
}