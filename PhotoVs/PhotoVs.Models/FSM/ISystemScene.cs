using PhotoVs.Models.ECS;

namespace PhotoVs.Models.FSM
{
    public interface ISystemScene : IScene
    {
        IGameObjectCollection Entities { get; }
        ISystemCollection Systems { get; }
    }
}