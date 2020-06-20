public class PluginTest : Plugin
{
    public string Name => "PluginTest";

    [GameEvent]
    [Trigger(EventType.GAME_START)]
    [RunOnce]
    void UseAControllerDummy(IGameEventArgs args)
    {
        var sceneMachine = Services.Get<SceneMachine>();
        sceneMachine.Push(sceneMachine.ControllerRecommendationScreen);
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
        Spawn(SayHowLong(endTick - startTick));
    }

    IEnumerator SayHowLong(int ticks)
    {
        var player = Services.Get<Player>();
        var num = (ticks < 10000000000000) ? 0 : 1;

        player.LockMovement();
        yield return Dialogue("Debugger", "It took {# Yellow}" + ticks + " ticks{/#} to walk through.");
        yield return Move(GetGameObjectByName("Player"), new Vector2(0, 0), 100);
        player.UnlockMovement();

    }
}