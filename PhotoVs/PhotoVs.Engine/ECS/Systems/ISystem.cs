using System;

namespace PhotoVs.Models.ECS
{
    public interface ISystem
    {
        int Priority { get; set; }
        bool Active { get; set; }
        Type[] Requires { get; }
    }
}