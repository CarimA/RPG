using System;
using System.Collections.Generic;
using PhotoVs.Engine.ECS;

namespace PhotoVs.Engine.ECS.Components
{
    public class ComponentCollection : List<IComponent>, IComponentCollection
    {
        public T Get<T>() where T : IComponent
        {
            return (T) Find(Is<T>);
        }

        public bool TryGet<T>(out T component) where T : IComponent
        {
            component = Get<T>();
            return component != null;
        }

        public bool Has(Type type)
        {
            // todo: figure out a nice way of killing this lambda
            return Exists(component => component.GetType().IsAssignableFrom(type));
        }

        public bool Has<T>() where T : IComponent
        {
            return Exists(Is<T>);
        }

        public IEnumerable<Type> Types()
        {
            return ConvertAll(GetType);
        }

        private bool Is<T>(IComponent component) where T : IComponent
        {
            return component is T;
        }

        private Type GetType(IComponent component)
        {
            return component.GetType();
        }
    }
}