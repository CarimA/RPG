using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine;
using PhotoVs.Engine.ECS.GameObjects;
using PhotoVs.Engine.ECS.Systems;
using PhotoVs.Logic.Camera;
using PhotoVs.Logic.Collision;
using PhotoVs.Logic.Movement;
using PhotoVs.Logic.WorldZoning;
using PhotoVs.Engine.Assets;
using PhotoVs.Engine.ECS;
using PhotoVs.Engine.FSM;

namespace PhotoVs.Logic.Scenes
{
    public class OverworldScene : ISystemScene
    {
        private readonly SceneMachine _scene;
        private readonly World _world;

        public OverworldScene(SceneMachine scene)
        {
            _scene = scene;
            var events = _scene.Services.Get<Events>();
            var camera = _scene.Services.Get<SCamera>();
            var assetLoader = _scene.Services.Get<IAssetLoader>();
            var spriteBatch = _scene.Services.Get<SpriteBatch>();

            _world = new World(spriteBatch, assetLoader);
            _world.LoadMaps("maps\\");
            var mapBoundary = new SMapBoundaryGeneration(_world, camera);

            Entities = new GameObjectCollection();
            Systems = new SystemCollection<ISystem>
            {
                new SProcessMovement(),
                new SProcessVelocity(),
                mapBoundary,
                new SCollisionDebugRender(spriteBatch, assetLoader, mapBoundary,
                    camera),
                new SCollisionResolution(events, mapBoundary),
                new SProcessInteractionEvents(events, mapBoundary),
                new SMapRenderer(spriteBatch, assetLoader, mapBoundary,
                    camera)
            };
        }

        public bool IsBlocking { get; set; }
        public IGameObjectCollection Entities { get; }
        public ISystemCollection<ISystem> Systems { get; }

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