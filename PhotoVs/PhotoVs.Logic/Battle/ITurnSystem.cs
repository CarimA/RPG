using Microsoft.Xna.Framework;
using PhotoVs.Engine.ECS.GameObjects;
using PhotoVs.Engine.ECS.Systems;

namespace PhotoVs.Logic.Battle
{
    public interface ITurnSystem : ISystem
    {
        void BeforeTurn(IGameObjectCollection allGameObjects);
        void AfterTurn(IGameObjectCollection allGameObjects);
        void Update(GameTime gameTime, IGameObjectCollection allGameObjects);
        void Draw(GameTime gameTime, IGameObjectCollection allGameObjects);
    }
}
