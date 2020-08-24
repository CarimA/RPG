using System;
using System.Linq;
using MoonSharp.Interpreter;
using PhotoVs.Engine.Core;
using PhotoVs.Engine.Events.EventArgs;
using PhotoVs.Engine.Scripting;
using PhotoVs.Logic.PlayerData;

namespace PhotoVs.Logic.Modules
{
    public class EventTriggersModule
    {
        private readonly IInterpreter<Closure> _interpreter;
        private readonly Player _player;
        private readonly ISignal _signal;

        public EventTriggersModule(IInterpreter<Closure> interpreter, ISignal signal, GameState gameState)
        {
            _interpreter = interpreter;
            _signal = signal;
            _player = gameState.Player;

            interpreter.AddFunction("Subscribe", (Action<Table>) CreateEvent);
            interpreter.AddFunction("Notify", (Action<string>) Notify);
        }

        private void CreateEvent(Table table)
        {
            var triggers = (Table) table["Triggers"];
            var conditions = (Table) table["Conditions"];
            var runOnce = table["RunOnce"];
            var action = (Closure) table["Event"];

            if (triggers == null || triggers.Length == 0)
                throw new ArgumentException("Triggers cannot be empty");

            if (action == null)
                throw new ArgumentException("Event cannot be empty");

            var runOnceBool = runOnce as bool? ?? false;
            var runOnceKey = runOnce != null
                ? runOnce is bool ? string.Empty : ((DynValue) runOnce).String
                : string.Empty;

            foreach (var trigger in triggers.Values)
            {
                var key = trigger.String;
                RegisterSignal(key, conditions, action, runOnceBool, runOnceKey);
            }
        }

        private void RegisterSignal(string signal, Table conditions, Closure closure,
            bool runOnce = false, string runOnceFlag = "")
        {
            var reservedId = _signal.ReserveId();

            var action = new Action<IGameEventArgs>(obj =>
            {
                // condition checks first
                if (conditions != null)
                    if (conditions.Values.Select(condition => condition.Function)
                        .Any(cs => Equals(cs.Call(), DynValue.False)))
                        return;

                if (runOnce && !string.Equals(runOnceFlag, string.Empty))
                    if (_player.PlayerData.GetFlag(runOnceFlag))
                        return;

                _interpreter.RunCoroutine(
                    $"event subscribed to ({signal})", closure);

                if (runOnce)
                {
                    _signal.Unsubscribe(reservedId);

                    if (!string.Equals(runOnceFlag, string.Empty))
                        _player.PlayerData.SetFlag(runOnceFlag, true);
                }
            });

            _signal.Subscribe(reservedId, signal, action);
        }

        public void Notify(string signal)
        {
            _signal.Notify(signal, null);
        }
    }
}