using System;

namespace PhotoVs.Engine.ECS.Systems
{
    public interface ISystem
    {
        int Priority { get; set; }
        bool Active { get; set; }
        Type[] Requires { get; }
    }
}