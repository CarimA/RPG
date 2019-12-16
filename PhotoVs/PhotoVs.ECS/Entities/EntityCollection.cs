using System;
using System.Collections.Generic;
using System.Linq;
using PhotoVs.Core.ECS.Entities;

namespace PhotoVs.ECS.Entities
{
    // todo: oh no
    public class EntityCollection : List<IEntity>
    {
        private int _hash = 487;

        public EntityCollection()
        {
        }

        public EntityCollection(List<IEntity> collection) : base(collection)
        {
            collection.ForEach(Reseed);
        }

        public IEntity this[string name]
        {
            get { return Find(entity => entity.Name == name); }
        }

        private void Reseed(IEntity entity)
        {
            unchecked
            {
                _hash = _hash * 31 + entity.GetHashCode();
            }
        }

        public new void Add(IEntity entity)
        {
            Reseed(entity);
            base.Add(entity);
        }

        public new void Remove(IEntity entity)
        {
            Reseed(entity);
            base.Remove(entity);
        }

        public int GetUniqueSeed()
        {
            return _hash;
        }

        public EntityCollection FindByTag(string tag)
        {
            return new EntityCollection(FindAll(entity => entity.Tags.Contains(tag)));
        }

        public EntityCollection Any(params Type[] types)
        {
            return new EntityCollection(FindAll(entity => types.Any(entity.Components.Has)));
        }

        public EntityCollection Except(params Type[] types)
        {
            return new EntityCollection(FindAll(entity => types.All(type => !entity.Components.Has(type))));
        }

        public EntityCollection All(params Type[] types)
        {
            return new EntityCollection(FindAll(entity => types.All(entity.Components.Has)));
        }
    }
}