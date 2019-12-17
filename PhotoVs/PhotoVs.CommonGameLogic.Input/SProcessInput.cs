using System;
using Microsoft.Xna.Framework;
using PhotoVs.ECS.Entities;
using PhotoVs.ECS.Systems;

namespace PhotoVs.CommonGameLogic.Input
{
    public class SProcessInput : IUpdateableSystem
    {
        public int Priority { get; set; } = -999;
        public bool Active { get; set; } = true;
        public Type[] Requires { get; } = {typeof(CInput)};

        public void BeforeUpdate(GameTime gameTime)
        {
        }

        public void Update(GameTime gameTime, EntityCollection entities)
        {
            foreach (var entity in entities)
            {
                var input = entity.Components.Get<CInput>();
                input.Input.Update(gameTime);
            }
        }

        public void AfterUpdate(GameTime gameTime)
        {
        }
    }
}