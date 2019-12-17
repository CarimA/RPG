using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using PhotoVs.CommonGameLogic.Input;
using PhotoVs.CommonGameLogic.Transforms;
using PhotoVs.ECS.Entities;
using PhotoVs.ECS.Systems;
using PhotoVs.GameInput;
using PhotoVs.PlayerData;

namespace PhotoVs.CommonGameLogic.Movement
{
    public class SProcessVelocity : IUpdateableSystem
    {
        public int Priority { get; set; } = 0;
        public bool Active { get; set; } = true;
        public Type[] Requires { get; } = { typeof(CPosition), typeof(CVelocity) };

        public void BeforeUpdate(GameTime gameTime)
        {
        }

        public void Update(GameTime gameTime, EntityCollection entities)
        {
            foreach (var entity in entities)
            {
                var position = entity.Components.Get<CPosition>();
                var velocity = entity.Components.Get<CVelocity>().Velocity;

                position.Position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public void AfterUpdate(GameTime gameTime)
        {
        }
    }
}
