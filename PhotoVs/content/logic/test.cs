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
            var events = services.Get<EventQueue>();
            _scheduler = services.Get<Scheduler>();
            _db = services.Get<TextDatabase>();
            _camera = services.Get<SCamera>();
            _player = services.Get<Player>();

            //eventsMan.OnInteractEventEnter["example_event"] += InteractEventHandler;
            int startTick = 0;

            events.Subscribe(EventType.INTERACT_AREA_ENTER + ":example_event", args =>
            {
                if (!(args is InteractEventArgs interactEventArgs))
                    return;

                startTick = Environment.TickCount;
            });

            events.Subscribe(EventType.INTERACT_AREA_EXIT + ":example_event", args =>
            {
                if (!(args is InteractEventArgs interactEventArgs))
                    return;

                var endTick = Environment.TickCount;

                _scheduler.Spawn(TellHowLong(endTick - startTick));
            });
        }

        private void InteractEventHandler(IGameObject player, IGameObject script)
        {
            _scheduler.Spawn(_scheduler.LockMovement(DoThis));
        }

        private IEnumerator TellHowLong(int ticks)
        {
            _player.LockMovement();
            yield return _scheduler.Dialogue("Debugger", "It took {# Yellow}" + ticks + " ticks{/#} to walk through.");
            _player.UnlockMovement();
        }

        private IEnumerator DoThis()
        {
            yield return _scheduler.Dialogue("Test Name", "I don't really have much to say.\nHere's a line.\nAnd another line.\nAnd another one?!");
        }
    }