using System;

namespace PhotoVs.ECS.Systems
{
    public interface ISystem
    {
        int Priority { get; set; }
        bool Active { get; set; }
        Type[] Requires { get; }
    }
}