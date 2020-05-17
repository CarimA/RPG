using System.Collections.Generic;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Engine.ECS.Systems
{
    public class SystemCollection<T> : List<T>, ISystemCollection<T> where T : ISystem
    {
        private int _hash = 487;

        public SystemCollection()
        {
        }

        public SystemCollection(IEnumerable<T> collection) : base(collection)
        {
            collection.ForEach(Reseed);
        }

        public new void Add(T system)
        {
            Reseed(system);
            base.Add(system);
        }

        public new void Remove(T system)
        {
            Reseed(system);
            base.Remove(system);
        }

        private void Reseed(T system)
        {
            unchecked
            {
                _hash = _hash * 31 + system.GetHashCode();
            }
        }

        public int GetUniqueSeed()
        {
            return _hash;
        }
    }
}