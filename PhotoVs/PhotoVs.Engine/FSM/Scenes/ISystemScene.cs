using PhotoVs.Engine.ECS;

namespace PhotoVs.Engine.FSM
{
    public interface ISystemScene : IScene
    {
        IGameObjectCollection Entities { get; }
        ISystemCollection Systems { get; }
    }
}