using PhotoVs.Engine;
using PhotoVs.Engine.ECS;
using PhotoVs.Logic.Mechanics.World.Systems;

namespace PhotoVs.Logic.NewScenes.GameScenes
{
    public class WorldScene : Scene
    {
        public override bool IsBlocking => true;

        public WorldScene(Services services)
            : base(new SystemList()
            {
                new SRenderOverworld(services)
            })
        {

        }
    }
}
