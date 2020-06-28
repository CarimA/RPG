using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PhotoVs.Engine.ECS
{
    public class GameObjectList : IList<GameObject>
    {
        private readonly List<GameObject> _gameObjects;
        public int Count => _gameObjects.Count;
        public bool IsReadOnly => false;

        public GameObjectList()
        {
            _gameObjects = new List<GameObject>();
        }

        public GameObjectList(List<GameObject> gameObjects)
        {
            _gameObjects = gameObjects;
        }

        public GameObject this[string id] => _gameObjects.Find(gameObject => gameObject.ID.Equals(id));

        public void Add(GameObject gameObject)
        {
            if (gameObject == null)
                throw new ArgumentNullException(nameof(gameObject));

            _gameObjects.Add(gameObject);
        }

        public void Clear()
        {
            _gameObjects.Clear();
        }

        public bool Contains(GameObject item)
        {
            return _gameObjects.Contains(item);
        }

        public void CopyTo(GameObject[] array, int arrayIndex)
        {
            _gameObjects.CopyTo(array, arrayIndex);
        }

        public void AddRange(IEnumerable<GameObject> gameObjects)
        {
            foreach (var gameObject in gameObjects)
                Add(gameObject);
        }

        public bool Remove(GameObject gameObject)
        {
            if (gameObject == null)
                throw new ArgumentNullException(nameof(gameObject));

            return _gameObjects.Remove(gameObject);
        }

        private GameObjectList ApplyLinq(Predicate<GameObject> predicate)
        {
            return new GameObjectList(_gameObjects.FindAll(gameObject => gameObject.Enabled && predicate(gameObject)));
        }

        public GameObjectList All(params Type[] types)
        {
            return ApplyLinq(obj => types.All(obj.Components.Has));
        }

        public GameObjectList Any(params Type[] types)
        {
            return ApplyLinq(obj => types.Any(obj.Components.Has));
        }

        public GameObjectList Except(params Type[] types)
        {
            return ApplyLinq(obj => types.All(type => !obj.Components.Has(type)));
        }

        public GameObjectList HasTag(string tag)
        {
            return ApplyLinq(obj => obj.Tags.Contains(tag));
        }

        public GameObject FindByName(string name)
        {
            return _gameObjects.Find(gameObject => gameObject.Name.Equals(name));
        }

        public IEnumerator<GameObject> GetEnumerator()
        {
            return _gameObjects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int IndexOf(GameObject item)
        {
            return _gameObjects.IndexOf(item);
        }

        public void Insert(int index, GameObject item)
        {
            _gameObjects.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _gameObjects.RemoveAt(index);
        }

        public GameObject this[int index]
        {
            get => _gameObjects[index];
            set => _gameObjects[index] = value;
        }
    }
}