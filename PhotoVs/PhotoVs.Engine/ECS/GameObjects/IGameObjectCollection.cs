using System;
using System.Collections.Generic;

namespace PhotoVs.Engine.ECS.GameObjects
{
    public interface IGameObjectCollection : IList<GameObject>
    {
        GameObject this[string name] { get; }

        IGameObjectCollection All(params Type[] types);
        IGameObjectCollection Any(params Type[] types);
        IGameObjectCollection Except(params Type[] types);
        IGameObjectCollection FindByTag(string tag);
        GameObject FindById(string id);
    }
}