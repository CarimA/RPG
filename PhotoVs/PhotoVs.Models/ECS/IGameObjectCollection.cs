using System;
using System.Collections.Generic;

namespace PhotoVs.Models.ECS
{
    public interface IGameObjectCollection : IList<IGameObject>
    {
        IGameObject this[string name] { get; }

        void Add(IGameObject entity);
        IGameObjectCollection All(params Type[] types);
        IGameObjectCollection Any(params Type[] types);
        IGameObjectCollection Except(params Type[] types);
        IGameObjectCollection FindByTag(string tag);
        void Remove(IGameObject entity);
    }
}