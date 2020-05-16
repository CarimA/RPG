using System.Collections.Generic;

namespace PhotoVs.Engine.ECS
{
    public interface ISystemCollection<T> : IList<T> where T : ISystem
    {
    }
}