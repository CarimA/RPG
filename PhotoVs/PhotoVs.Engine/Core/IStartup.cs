using System.Collections.Generic;

namespace PhotoVs.Engine.Core
{
    public interface IStartup
    {
        void Start(IEnumerable<object> bindings);
    }
}