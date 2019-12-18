using System;
using System.Collections.Generic;

namespace PhotoVs.Engine
{
    [Serializable]
    public class IndexedEvent<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public new TValue this[TKey key]
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