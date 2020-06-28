using System;
using System.Collections.Generic;

namespace PhotoVs.Engine.ECS
{
    public class ComponentList
    {
        private Dictionary<Type, object> _enabledComponents;
        private Dictionary<Type, object> _disabledComponents;

        public ComponentList()
        {
            _enabledComponents = new Dictionary<Type, object>();
            _disabledComponents = new Dictionary<Type, object>();
        }

        public void Add<T>(T component) where T : class
        {
            if (!typeof(T).IsClass)
                throw new ArgumentException("Component provided is not a class");

            _enabledComponents[typeof(T)] = component
                                            ?? throw new ArgumentNullException(nameof(component));
        }

        public bool Remove<T>() where T : class
        {
            return (_enabledComponents.Remove(typeof(T))
                    || _disabledComponents.Remove(typeof(T)));
        }

        public T Get<T>() where T : class
        {
            if (_enabledComponents.TryGetValue(typeof(T), out var result))
                return (T)result;
            return null;
        }

        public bool TryGet<T>(out T component) where T : class
        {
            component = Get<T>();
            return component != null;
        }

        public bool Has<T>() where T : class
        {
            return _enabledComponents.ContainsKey(typeof(T));
        }

        public bool Has(Type type)
        {
            return _enabledComponents.ContainsKey(type);
        }
        public IEnumerable<Type> GetTypes()
        {
            foreach (var enabled in _enabledComponents)
                yield return enabled.Key;

            foreach (var disabled in _disabledComponents)
                yield return disabled.Key;
        }

        public bool Enable<T>() where T : class
        {
            var type = typeof(T);
            if (!_disabledComponents.ContainsKey(type))
                return false;

            _enabledComponents[type] = _disabledComponents[type];
            _disabledComponents.Remove(type);
            return true;
        }

        public bool Disable<T>() where T : class
        {
            var type = typeof(T);
            if (!_enabledComponents.ContainsKey(type))
                return false;

            _disabledComponents[type] = _enabledComponents[type];
            _enabledComponents.Remove(type);
            return true;
        }
    }
}
