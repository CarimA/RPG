using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PhotoVs.Engine.ECS.Systems;

namespace PhotoVs.Engine.ECS
{
    public class SystemList : List<ISystem>
    {
        public new void Add(ISystem system)
        {
            if (system == null)
                throw new ArgumentNullException(nameof(system));

            base.Add(system);
            Sort(SortByPriority);
        }

        private int SortByPriority(ISystem a, ISystem b)
        {
            return a.Priority.CompareTo(b.Priority);
        }

        public new void AddRange(IEnumerable<ISystem> systems)
        {
            foreach (var system in systems)
                Add(system);
        }

    }
}