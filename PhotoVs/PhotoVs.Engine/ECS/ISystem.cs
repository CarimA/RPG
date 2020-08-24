using System;

namespace PhotoVs.Engine.ECS
{
    public interface ISystem
    {
        bool Enabled { get; set; }
        Type[] RequiredComponents { get; }
        int Priority { get; }
        RunOn RunOn { get; }
    }
}