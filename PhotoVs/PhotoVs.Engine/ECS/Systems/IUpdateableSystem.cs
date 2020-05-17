using Microsoft.Xna.Framework;
using PhotoVs.Engine.ECS.GameObjects;

namespace PhotoVs.Engine.ECS.Systems
{
    public interface IUpdateableSystem : ISystem
    {
        void BeforeUpdate(GameTime gameTime);
        void Update(GameTime gameTime, IGameObjectCollection entities);
        void AfterUpdate(GameTime gameTime);
    }
}