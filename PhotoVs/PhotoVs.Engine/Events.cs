using System;
using PhotoVs.Models.ECS;

namespace PhotoVs.Engine
{
    public sealed class Events
    {
        public delegate void CollisionEventHandler(IGameObject moving, IGameObject stationary);

        public event EventHandler OnGameStart;
        public event CollisionEventHandler OnCollision;

        public void RaiseOnCollision(IGameObject moving, IGameObject stationary)
        {
            OnCollision?.Invoke(moving, stationary);
        }
    }
}