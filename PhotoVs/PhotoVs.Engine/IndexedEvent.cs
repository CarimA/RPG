using System;
using System.Collections.Generic;

namespace PhotoVs.Engine
{
    [Serializable]
    public class IndexedEvent<T> : Dictionary<string, T>
    {
        public new T this[string key]
        {
            get
            {
                if (ContainsKey(key))
                {
                    return base[key];
                }

                Add(key, default);
                return base[key];
            }
            set => base[key] = value;
        }
    }
}