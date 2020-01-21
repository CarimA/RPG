using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoVs.Logic.Battles
{
    public class BattlePhoton
    {
        private int _speed;
        public int Speed => _speed;

        private int _priority;
        public int Priority => _priority;

        private bool _hasMoved;
        public bool HasMoved => _hasMoved;

        private bool _canAttack;
        public bool CanAttack => _canAttack;

        public BattlePhoton()
        {
            _speed = 0;
            _priority = 0;
            _hasMoved = false;
            _canAttack = true;
        }
    }
}
