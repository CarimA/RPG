using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoVs.Engine.ECS;
using PhotoVs.Engine.ECS.Components;

namespace PhotoVs.Logic.Battle.Components
{
    public class CGridPosition : IComponent
    {
        public int X;
        public int Y;
    }
}
