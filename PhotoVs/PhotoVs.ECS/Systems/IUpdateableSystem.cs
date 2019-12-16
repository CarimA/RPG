using Microsoft.Xna.Framework;
using PhotoVs.ECS.Entities;

namespace PhotoVs.ECS.Systems
{
    public interface IUpdateableSystem : ISystem
    {
        void BeforeUpdate(GameTime gameTime);
        void Update(GameTime gameTime, EntityCollection entities);
        void AfterUpdate(GameTime gameTime);
    }
}