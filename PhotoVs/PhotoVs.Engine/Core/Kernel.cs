using System;
using System.Collections.Generic;
using System.Linq;
using PhotoVs.Utils.Logging;

namespace PhotoVs.Engine.Core
{
    public class Kernel : IKernel, IDisposable
    {
        private readonly List<(Type, Type)> _injectingTypes;
        private readonly Dictionary<Type, object> _instances;

        public Kernel()
        {
            _instances = new Dictionary<Type, object>();
            _injectingTypes = new List<(Type, Type)>();
        }

        public void Dispose()
        {
            foreach (var item in _instances.Values)
                if (item is IDisposable disposable)
                    disposable.Dispose();
        }

        public Action<object> OnBind { get; set; }
        public Action OnConstruct { get; set; }

        [Obsolete("This is only allowed to be used with build tools, ask for objects in constructors instead.")]
        public T Find<T>()
        {
            if (_instances.TryGetValue(typeof(T), out var value))
                return (T)value;

            throw new KeyNotFoundException(typeof(T).Name);
        }

        public virtual Kernel Bind<T>(T instance) where T : class
        {
            return Bind(typeof(T), instance);
        }

        public virtual Kernel Bind(Type type, object instance)
        {
            _instances.Add(type, instance);
            OnBind?.Invoke(instance);
            Logger.Write.Trace($"Registered Type \"{type.Name}\" as service.");
            return this;
        }

        public virtual Kernel Bind<TInterface, TObject>()
            where TInterface : class
            where TObject : TInterface
        {
            _injectingTypes.Add((typeof(TInterface), typeof(TObject)));
            return this;
        }

        public virtual void Construct()
        {
            var sorted = new List<(Type, Type)>();
            var visited = new HashSet<(Type, Type)>();

            // sort by dependency
            foreach (var item in _injectingTypes)
                Visit(item, visited, sorted);

            // initialise each one
            foreach (var item in sorted)
            {
                var interfaceType = item.Item1;
                var objectType = item.Item2;

                var requires = objectType
                    .GetConstructors()[0]
                    .GetParameters()
                    .Select(param => param.ParameterType);

                var obj = Activator.CreateInstance(item.Item2, requires.Select(t => _instances[t]).ToArray());
                Bind(interfaceType, obj);
            }

            _injectingTypes.Clear();
            OnConstruct?.Invoke();
        }

        public virtual Kernel Bind<T>()
            where T : class
        {
            return Bind<T, T>();
        }

        private void Visit(
            (Type, Type) item,
            HashSet<(Type, Type)> visited,
            List<(Type, Type)> sorted)
        {
            var interfaceType = item.Item1;
            var objectType = item.Item2;

            if (objectType.GetConstructors().Length > 1)
                throw new Exception($"{objectType.Name} has more than one constructor.");

            var requires = objectType
                .GetConstructors()[0]
                .GetParameters()
                .Select(param => param.ParameterType);

            if (!visited.Contains(item))
            {
                visited.Add(item);

                foreach (var dependency in requires)
                {
                    var nextObj = _injectingTypes.Find(obj => obj.Item1 == dependency);
                    if (nextObj == (null, null))
                    {
                        if (_instances.ContainsKey(dependency))
                        {
                            // it's fine, carry on.
                        }
                        else
                        {
                            throw new Exception(
                                $"{objectType.Name} contains a type ({dependency.Name}) that has not been bound.");
                        }
                    }
                    else
                    {
                        Visit(nextObj, visited, sorted);
                    }
                }

                sorted.Add(item);
            }
            else
            {
                if (!sorted.Contains(item))
                    throw new Exception($"{objectType.Name} has a cyclic dependency.");
            }
        }
    }
}