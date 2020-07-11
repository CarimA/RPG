using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoVs.Engine;
using PhotoVs.Engine.ECS;
using PhotoVs.Engine.ECS.Systems;
using PhotoVs.Logic.Mechanics.Movement.Systems;
using PhotoVs.Logic.Mechanics.World.Systems;

namespace PhotoVs.Logic.NewScenes.GameScenes
{
    public class WorldLogicScene : Scene
    {
        public override bool IsBlocking => false;

        public WorldLogicScene(Services services)
            : base(new SystemList()
            {
                new SCollisionDebugRender(services),
                new SCollisionResolution(services),
                new SProcessInteractionEvents(services)
            })
        {

        }
    }
}
