using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoVs.Engine.ECS;
using PhotoVs.Engine.ECS.GameObjects;
using PhotoVs.Engine.ECS.Systems;
using PhotoVs.Utils.Collections;

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
