using MoonSharp.Interpreter;
using PhotoVs.Engine.Events;
using PhotoVs.Engine.Events.EventArgs;
using PhotoVs.Engine.Scripting;
using PhotoVs.Logic.Events;
using System;
using System.Linq;
using PhotoVs.Logic.PlayerData;

namespace PhotoVs.Logic.Modules
{
    public class EventTriggersModule : Module
    {
        private readonly GameEventQueue _events;
        private readonly Player _player;

        public EventTriggersModule(GameEventQueue events, Player player)
        {
            _events = events;
            _player = player;
        }

        public override void DefineApi(MoonSharpInterpreter interpreter)
        {
            if (interpreter == null)
                throw new ArgumentNullException(nameof(interpreter));

            interpreter.AddFunction("Subscribe", (Action<Table>)CreateEvent);
            interpreter.AddFunction("Notify", (Action<GameEvents, string>)Notify);

            interpreter.AddType<GameEvents>("Events");

            interpreter.AddType<(GameEvents, string)>("_triggerTuple");
            interpreter.RegisterGlobal("Trigger", (Func<GameEvents, string, (GameEvents, string)>)Trigger);

            base.DefineApi(interpreter);
        }

        private (GameEvents, string) Trigger(GameEvents arg1, string arg2)
        {
            arg2 ??= string.Empty;
            return (arg1, arg2);
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
            var runOnceKey = runOnce != null ? (runOnce is bool ? string.Empty : ((DynValue)runOnce).String) : string.Empty;

            foreach (var trigger in triggers.Values)
            {
                if (trigger.UserData.Object.GetType() == typeof((GameEvents, string)))
                {
                    var (key, delimiter) = ((GameEvents, string))trigger.UserData.Object;
                    RegisterEvent(key, delimiter, conditions, action, runOnceBool, runOnceKey);
                }
                else
                {
                    RegisterEvent((GameEvents) trigger.Number, string.Empty, conditions, action, runOnceBool,
                        runOnceKey);
                }
            }
        }

        private void RegisterEvent(GameEvents gameEvent, string delimiter, Table conditions, Closure closure,
            bool runOnce = false, string runOnceFlag = "")
        {
            var reservedId = _events.ReserveId();

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

                ScriptHost.RunCoroutine(
                    $"event subscribed to ({Enum.GetName(typeof(GameEvents), gameEvent)}, {delimiter})", closure);

                if (runOnce)
                {
                    _events.Unsubscribe(reservedId);

                    if (!string.Equals(runOnceFlag, string.Empty))
                        _player.PlayerData.SetFlag(runOnceFlag, true);
                }
            });

            _events.Subscribe(reservedId, gameEvent, delimiter, action);
        }

        public void Notify(GameEvents eventKey, string delimiter)
        {
            _events.Notify(eventKey, delimiter, null);
        }
    }
}
