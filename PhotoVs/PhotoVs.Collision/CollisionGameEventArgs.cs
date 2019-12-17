using PhotoVs.Core.ECS.Entities;
using PhotoVs.Events;

namespace PhotoVs.Collision
{
    public class CollisionGameEventArgs : IGameEventArgs
    {
        public IEntity Moving;
        public IEntity Stationary;

        public CollisionGameEventArgs(IEntity moving, IEntity stationary)
        {
            Moving = moving;
            Stationary = stationary;
        }
    }
}