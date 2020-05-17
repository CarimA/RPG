using PhotoVs.Engine.ECS.GameObjects;
using PhotoVs.Engine.ECS.Systems;

namespace PhotoVs.Engine.FSM.Scenes
{
    public interface ISystemScene : IScene
    {
        IGameObjectCollection Entities { get; }
        ISystemCollection<ISystem> Systems { get; }
    }
}