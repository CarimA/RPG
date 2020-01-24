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
    private ITextDatabase _db;
    private Player _player;

    public override string Name => "Test2";
    public override int Version => 1;

    // public override string Name { get; } = "Test Plugin";
    //public override int Version { get; } = 1;

    public TestPlugin(Services services)
    {
        var events = services.Get<Events>();

        events.OnInteractEventEnter["example_event"] += InteractEventHandler;

        _db = services.Get<ITextDatabase>();
        _camera = services.Get<SCamera>();
        _player = services.Get<Player>();
    }

    private void InteractEventHandler(object sender, IGameObject player, IGameObject script)
    {
        Spawn(LockMovement(DoThis));
    }

    private IEnumerator DoThis()
    {
        _camera.Set(new List<Vector2>
                {
                    new Vector2(-600, -600),
                    new Vector2(600, 600)
                });

        var text = TextInput("Hi", "Test", 10);
        yield return text;

        yield return Spawn(WaitTest());
        Logger.Write.Trace("Test 1");

        //yield return Dialogue("test", $"Hello {text.Text}!");
        yield return Dialogue("test", _db.GetText("Intro"));
        Logger.Write.Trace("Test 2");
        yield return Pause(3f);
        Logger.Write.Trace("Test 3");

        _camera.Follow(_player);
    }

    private IEnumerator WaitTest()
    {
        yield return Pause(3f);
    }
}