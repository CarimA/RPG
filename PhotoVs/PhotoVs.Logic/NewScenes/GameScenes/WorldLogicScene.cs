using PhotoVs.Engine;
using PhotoVs.Engine.ECS;
using PhotoVs.Logic.Mechanics.Movement.Systems;

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
