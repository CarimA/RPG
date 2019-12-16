﻿using System.Collections.Generic;
using PhotoVs.Extensions;

namespace PhotoVs.ECS.Systems
{
    public class SystemCollection : List<ISystem>
    {
        private int _hash = 487;

        public SystemCollection()
        {
        }

        public SystemCollection(IEnumerable<ISystem> collection) : base(collection)
        {
            collection.ForEach(Reseed);
        }

        private void Reseed(ISystem system)
        {
            unchecked
            {
                _hash = _hash * 31 + system.GetHashCode();
            }
        }

        public new void Add(ISystem system)
        {
            Reseed(system);
            base.Add(system);
        }

        public new void Remove(ISystem system)
        {
            Reseed(system);
            base.Remove(system);
        }

        public int GetUniqueSeed()
        {
            return _hash;
        }
    }
}