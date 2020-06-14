using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.ECS.GameObjects;
using PhotoVs.Engine.ECS.Systems;
using PhotoVs.Engine.FSM.Scenes;
using PhotoVs.Logic.Mechanics.Camera.Systems;
using PhotoVs.Logic.Mechanics.Movement.Systems;
using PhotoVs.Logic.Mechanics.World;
using PhotoVs.Logic.Mechanics.World.Systems;

namespace PhotoVs.Logic.Scenes
{
    public class OverworldScene : ISystemScene
    {
        private readonly SceneMachine _scene;
        private readonly Overworld _world;

        public OverworldScene(SceneMachine scene)
        {
            _scene = scene;
            var events = _scene.Services.Get<Events>();
            var camera = _scene.Services.Get<SCamera>();
            var assetLoader = _scene.Services.Get<IAssetLoader>();
            var spriteBatch = _scene.Services.Get<SpriteBatch>();

            _world = new Overworld(spriteBatch, assetLoader);
            _world.LoadMaps("maps/");
            _world.SetMap("test");

            Entities = new GameObjectCollection();
            Systems = new SystemCollection<ISystem>
            {
                new SRenderOverworld(_world, spriteBatch, camera),
                new SProcessMovement(),
                new SCollisionDebugRender(spriteBatch, assetLoader,
                    _world, camera),
                new SCollisionResolution(_world, camera, events),
                new SProcessInteractionEvents(events, camera, _world)
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