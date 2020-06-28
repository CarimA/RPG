using PhotoVs.Engine.ECS;
using PhotoVs.Engine.ECS.Systems;

namespace PhotoVs.Engine.FSM.Scenes
{
    public interface ISystemScene : IScene
    {
        GameObjectList Entities { get; }
        ISystemCollection<ISystem> Systems { get; }
    }
}