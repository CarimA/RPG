using System;
using System.Collections.Generic;
using System.Linq;

namespace PhotoVs.Engine.ECS.GameObjects
{
    // todo: oh no
    public class GameObjectCollection : List<IGameObject>, IGameObjectCollection
    {
        private int _hash = 487;

        public GameObjectCollection()
        {
        }

        public GameObjectCollection(List<IGameObject> collection) : base(collection)
        {
            collection.ForEach(Reseed);
        }

        public IGameObject this[string name]
        {
            get { return Find(entity => entity.Name == name); }
        }

        public new void Add(IGameObject entity)
        {
            Reseed(entity);
            base.Add(entity);
        }

        public IGameObjectCollection FindByTag(string tag)
        {
            return new GameObjectCollection(FindAll(entity => entity.Tags.Contains(tag)));
        }

        public IGameObject FindById(string id)
        {
            return Find(entity => entity.ID == id);
        }

        public IGameObjectCollection Any(params Type[] types)
        {
            return new GameObjectCollection(FindAll(entity => types.Any(entity.Components.Has)));
        }

        public IGameObjectCollection Except(params Type[] types)
        {
            return new GameObjectCollection(FindAll(entity => types.All(type => !entity.Components.Has(type))));
        }

        public IGameObjectCollection All(params Type[] types)
        {
            return new GameObjectCollection(FindAll(entity => types.All(entity.Components.Has)));
        }

        public new void Remove(IGameObject entity)
        {
            Reseed(entity);
            base.Remove(entity);
        }

        private void Reseed(IGameObject entity)
        {
            unchecked
            {
                _hash = _hash * 31 + entity.GetHashCode();
            }
        }

        public int GetUniqueSeed()
        {
            return _hash;
        }
    }
}