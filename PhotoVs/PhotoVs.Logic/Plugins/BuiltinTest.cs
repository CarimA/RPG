using PhotoVs.Engine;
using PhotoVs.Engine.ECS;
using PhotoVs.Engine.Plugins;
using PhotoVs.Logic.PlayerData;
using PhotoVs.Logic.Scenes;
using PhotoVs.Logic.Text;
using System.Collections;
using PhotoVs.Engine.ECS.GameObjects;
using PhotoVs.Logic.Mechanics.Camera.Systems;

namespace PhotoVs.Logic.Plugins
{
    public class PluginTest : IPlugin
    {
        private readonly SceneMachine _sceneMachine;

        public string Name => "Test";
        public int Version => 1;

        public PluginTest(Services services)
        {
            _sceneMachine = services.Get<SceneMachine>();

            var events = services.Get<Events>();
            events.OnGameStart += EventsOnOnGameStart;
        }

        private void EventsOnOnGameStart()
        {
            _sceneMachine.Push(_sceneMachine.ControllerRecommendationScreen);
        }
    }

    public class TestPlugin : IPlugin
    {
        private Scheduler _scheduler;
        private SCamera _camera;
        private TextDatabase _db;
        private Player _player;

        public string Name => "Test2";
        public int Version => 55;

        public TestPlugin(Services services)
        {
            var events = services.Get<Events>();
            _scheduler = services.Get<Scheduler>();
            _db = services.Get<TextDatabase>();
            _camera = services.Get<SCamera>();
            _player = services.Get<Player>();

            events.OnInteractEventEnter["example_event"] += InteractEventHandler;
        }

        private void InteractEventHandler(IGameObject player, IGameObject script)
        {
            _scheduler.Spawn(_scheduler.LockMovement(DoThis));
        }

        private IEnumerator DoThis()
        {
            yield return _scheduler.Dialogue("Test Name", "I don't really have much to say.\nHere's a line.\nAnd another line.\nAnd another one?!");
        }
    }
}
