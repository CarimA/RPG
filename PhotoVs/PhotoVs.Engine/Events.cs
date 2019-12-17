﻿using PhotoVs.Models.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoVs.Engine
{
    public sealed class Events
    {
        public event EventHandler OnGameStart;

        public delegate void CollisionEventHandler(IGameObject moving, IGameObject stationary);
        public event CollisionEventHandler OnCollision;
        public void RaiseOnCollision(IGameObject moving, IGameObject stationary)
        {
            OnCollision?.Invoke(moving, stationary);
        }
    }
}