public class PluginTest : Plugin
{
    private readonly SceneMachine _sceneMachine;

    public override string Name => "Test";
    public override int Version => 1;

    public PluginTest(Services services)
    {
        _sceneMachine = services.Get<SceneMachine>();

        var events = services.Get<Events>();
        events.OnGameStart += EventsOnOnGameStart;
    }

    private void EventsOnOnGameStart(object sender)
    {
        _sceneMachine.Push(_sceneMachine.ControllerRecommendationScreen);
    }
}


public class TestPlugin : Plugin
{
    private SCamera _camera;
    private TextDatabase _db;
    private Player _player;

    public override string Name => "Test2";
    public override int Version => 55;

    public TestPlugin(Services services)
    {
        var events = services.Get<Events>();

        events.OnInteractEventEnter["example_event"] += InteractEventHandler;

        _db = services.Get<TextDatabase>();
        _camera = services.Get<SCamera>();
        _player = services.Get<Player>();
    }

    private void InteractEventHandler(object sender, IGameObject player, IGameObject script)
    {
        Spawn(LockMovement(DoThis));
    }

    private IEnumerator DoThis()
    {
        yield return Dialogue("Test Name", "I don't really have much to say.\nHere's a line.\nAnd another line.\nAnd another one?!");
    }
}