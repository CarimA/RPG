using PhotoVs.Utils.Logging;
using System;
using System.Collections.Generic;

namespace PhotoVs.Engine
{
    public class Services
    {
        private readonly Dictionary<Type, object> _cache;

        public Services()
        {
            _cache = new Dictionary<Type, object>();
        }

        public T Get<T>()
        {
            if (_cache.TryGetValue(typeof(T), out var value))
                return (T)value;

            throw new KeyNotFoundException();
        }

        public void Set<T>(T service)
        {
            _cache.Add(typeof(T), service);
            Logger.Write.Trace($"Registered Type \"{typeof(T).Name}\" as service.");
        }
    }
}