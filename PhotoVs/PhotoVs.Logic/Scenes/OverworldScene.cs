using PhotoVs.Engine.ECS.GameObjects;
using PhotoVs.Engine.ECS.Systems;
using PhotoVs.Logic.Collision;
using PhotoVs.Logic.Movement;
using PhotoVs.Logic.WorldZoning;
using PhotoVs.Models.ECS;
using PhotoVs.Models.FSM;

namespace PhotoVs.Logic.Scenes
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

            Entities = new GameObjectCollection
            {
                new GameObject()
            };
            Systems = new SystemCollection
            {
                new SProcessMovement(),
                new SProcessVelocity(),
                mapBoundary,
                new SCollisionDebugRender(scene.SpriteBatch, scene.AssetLoader, mapBoundary, scene.Camera),
                new SCollisionResolution(scene.GameEvents, mapBoundary),
                new SProcessInteractionEvents(scene.GameEvents, mapBoundary),
                new SMapRenderer(scene.SpriteBatch, scene.AssetLoader, mapBoundary, scene.Camera)
            };
        }

        public bool IsBlocking { get; set; }
        public IGameObjectCollection Entities { get; }
        public ISystemCollection Systems { get; }

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