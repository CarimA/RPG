using Microsoft.Xna.Framework;
using PhotoVs.Engine.ECS.GameObjects;
using PhotoVs.Engine.ECS.Systems;

namespace PhotoVs.Logic.Battle
{
    public interface IBattleActionSystem : ISystem
    {
        void BeforeAction(IGameObject actionGameObject, IGameObjectCollection allGameObjects);
        void AfterAction(IGameObject actionGameObject, IGameObjectCollection allGameObjects);
        bool Update(GameTime gameTime, GameObject active, IGameObjectCollection allGameObjects);
        void Draw(GameTime gameTime, GameObject active, IGameObjectCollection allGameObjects);
    }
}
