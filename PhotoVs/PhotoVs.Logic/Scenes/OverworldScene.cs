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

            _world = new World(scene.Services.SpriteBatch, scene.Services.AssetLoader);
            _world.LoadMaps("maps\\");
            var mapBoundary = new SMapBoundaryGeneration(_world, scene.Services.Camera);

            Entities = new GameObjectCollection();
            Systems = new SystemCollection
            {
                new SProcessMovement(),
                new SProcessVelocity(),
                mapBoundary,
                new SCollisionDebugRender(scene.Services.SpriteBatch, scene.Services.AssetLoader, mapBoundary,
                    scene.Services.Camera),
                new SCollisionResolution(scene.Services.Events, mapBoundary),
                new SProcessInteractionEvents(scene.Services.Events, mapBoundary),
                new SMapRenderer(scene.Services.SpriteBatch, scene.Services.AssetLoader, mapBoundary,
                    scene.Services.Camera)
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

        public void PushDialogue(string name, string dialogue)
        {
            _scene.Push(_scene.DialogueScene, name, dialogue);
        }

        public void PushTextInputScene(string question, string defaultText = "", int limit = 15)
        {
            _scene.Push(_scene.TextInputScene, question, limit, defaultText);
        }
    }
}