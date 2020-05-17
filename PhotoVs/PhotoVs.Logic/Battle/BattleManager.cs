using PhotoVs.Engine.ECS.GameObjects;
using PhotoVs.Engine.ECS.Systems;

namespace PhotoVs.Logic.Battle
{
    public class BattleManager
    {
        private readonly ISystemCollection<IBattleActionSystem> _battleActionSystems;
        private readonly ISystemCollection<ITurnSystem> _turnSystems;
        private readonly IGameObjectCollection _battleEntities;

        public BattleManager()
        {
            _battleActionSystems = new SystemCollection<IBattleActionSystem>();
            _turnSystems = new SystemCollection<ITurnSystem>();
            _battleEntities = new GameObjectCollection();
        }
    }
}
