using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoVs.ECS.Components;

namespace PhotoVs.WorldZoning
{
    public class CZone : IComponent
    {
        public string Name;

        public CZone(string name)
        {
            Name = name;
        }
    }
}