public class PluginTest : IPlugin
{
    public string Name => "Test";

    private readonly EventCommands EventCommands;

    private readonly SceneMachine _sceneMachine;
    private readonly EventQueue _events;
    private readonly Player _player;
    
    public PluginTest(Services services)
    {
        EventCommands = services.Get<EventCommands>();

        _sceneMachine = services.Get<SceneMachine>();
        _player = services.Get<Player>();
        _events = services.Get<EventQueue>();
    }

    [GameEvent]
    [Trigger(EventType.GAME_START)]
    [RunOnce]
    void UseAControllerDummy(IGameEventArgs args)
    {
        _sceneMachine.Push(_sceneMachine.ControllerRecommendationScreen);
    }

    int startTick = 0;

    [GameEvent]
    [Trigger(EventType.INTERACT_AREA_ENTER + ":example_event")]
    void StartTrackingTimeToWalk(IGameEventArgs args)
    {
        startTick = Environment.TickCount;
    }

    [GameEvent]
    [Trigger(EventType.INTERACT_AREA_EXIT + ":example_event")]
    void StopTrackingTimeToWalk(IGameEventArgs args)
    {
        var endTick = Environment.TickCount;
        EventCommands.Spawn(SayHowLong(endTick - startTick));
    }

    IEnumerator SayHowLong(int ticks)
    {
        _player.LockMovement();
        yield return EventCommands.Dialogue("Debugger", "It took {# Yellow}" + ticks + " ticks{/#} to walk through.");
        _player.UnlockMovement();
    }
}