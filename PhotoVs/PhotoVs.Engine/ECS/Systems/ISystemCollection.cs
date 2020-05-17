using System.Collections.Generic;

namespace PhotoVs.Engine.ECS.Systems
{
    public interface ISystemCollection<T> : IList<T> where T : ISystem
    {
    }
}