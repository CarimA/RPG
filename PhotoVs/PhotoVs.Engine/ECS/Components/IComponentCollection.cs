using System;
using System.Collections.Generic;

namespace PhotoVs.Engine.ECS
{
    public interface IComponentCollection : IList<IComponent>
    {
        T Get<T>() where T : IComponent;
        bool Has(Type type);
        bool Has<T>() where T : IComponent;
        bool TryGet<T>(out T component) where T : IComponent;
        IEnumerable<Type> Types();
    }
}