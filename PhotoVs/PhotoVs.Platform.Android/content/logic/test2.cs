public class PluginTest : IPlugin
{
    private readonly SceneMachine _sceneMachine;
    private readonly EventQueue _events;

    public string Name => "Test";
    public int Version => 1;

    private string gameStartId;
    
    public PluginTest(Services services)
    {
        _sceneMachine = services.Get<SceneMachine>();

        _events = services.Get<EventQueue>();
    }

    [Trigger(EventType.GAME_START)]
    private void EventsOnOnGameStart(IGameEventArgs gameEventArgs)
    {
        _sceneMachine.Push(_sceneMachine.ControllerRecommendationScreen);
        _events.Unsubscribe(gameStartId);
    }
}