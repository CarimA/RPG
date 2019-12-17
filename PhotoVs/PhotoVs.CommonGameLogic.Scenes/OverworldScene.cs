using PhotoVs.Collision;
using PhotoVs.CommonGameLogic.Movement;
using PhotoVs.Core.ECS.Entities;
using PhotoVs.ECS.Entities;
using PhotoVs.ECS.Systems;
using PhotoVs.FSM.Scenes;
using PhotoVs.WorldZoning;

namespace PhotoVs.CommonGameLogic.Scenes
{
    public class OverworldScene : ISystemScene
    {
        private readonly SceneMachine _scene;
        private readonly World _world;

        public OverworldScene(SceneMachine scene)
        {
            _scene = scene;

            _world = new World(scene.SpriteBatch, scene.AssetLoader);
            _world.LoadMaps("maps\\");
            var mapBoundary = new SMapBoundaryGeneration(_world, scene.Camera);

            Entities = new EntityCollection
            {
                new Entity()
            };
            Systems = new SystemCollection
            {
                new SProcessMovement(),
                new SProcessVelocity(),
                mapBoundary,
                new SCollisionDebugRender(scene.SpriteBatch, scene.AssetLoader, mapBoundary, scene.Camera),
                new SCollisionResolution(scene.GameEvents, mapBoundary),
                //new SMapInteraction(scene.Engine, mapBoundary),
                new SMapRenderer(scene.SpriteBatch, scene.AssetLoader, mapBoundary, scene.Camera)
            };
        }

        public bool IsBlocking { get; set; }
        public EntityCollection Entities { get; }
        public SystemCollection Systems { get; }

        public void Enter(params object[] args)
        {
        }

        public void Exit()
        {
        }

        public void Resume()
        {
        }

        public void Suspend()
        {
        }
    }
}