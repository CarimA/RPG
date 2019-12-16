using PhotoVs.ECS.Entities;
using PhotoVs.ECS.Systems;

namespace PhotoVs.FSM.Scenes
{
    public interface ISystemScene : IScene
    {
        EntityCollection Entities { get; }
        SystemCollection Systems { get; }
    }
}