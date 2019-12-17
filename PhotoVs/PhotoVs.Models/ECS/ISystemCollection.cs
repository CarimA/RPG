using System.Collections.Generic;

namespace PhotoVs.Models.ECS
{
    public interface ISystemCollection : IList<ISystem>
    {
        void Add(ISystem system);
        void Remove(ISystem system);
    }
}