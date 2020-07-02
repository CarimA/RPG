using MoonSharp.Interpreter;
using PhotoVs.Engine.Events;
using PhotoVs.Engine.Events.EventArgs;
using PhotoVs.Engine.Scripting;
using PhotoVs.Logic.Events;
using System;
using System.Linq;

namespace PhotoVs.Logic.Modules
{
    public class EventTriggersModule : Module
    {
        private readonly EventQueue<GameEvents> _events;

        public EventTriggersModule(EventQueue<GameEvents> events)
        {
            _events = events;
        }

        public override void DefineApi(MoonSharpInterpreter interpreter)
        {
            if (interpreter == null)
                throw new ArgumentNullException(nameof(interpreter));

            interpreter.AddFunction("Subscribe", (Action<object, object, object, object, object>)CreateEvent);
            interpreter.AddFunction("_Trigger", (Func<GameEvents, string, (GameEvents, string)>)Trigger);
            interpreter.AddType<GameEvents>("Events");
            interpreter.RegisterGlobal("RunOnce", true);

            base.DefineApi(interpreter);
        }

        private (GameEvents, string) Trigger(GameEvents arg1, string arg2)
        {
            arg2 ??= string.Empty;
            return (arg1, arg2);
        }

        private void CreateEvent(object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            if (arg1 is GameEvents gameEvents1 && arg2 is Closure closure1)
                RegisterEvent(gameEvents1, string.Empty, null, closure1, (bool?)arg3 ?? false);

            else if (arg1 is GameEvents gameEvents2 && arg2 is string delimiter2 && arg3 is Closure closure2)
                RegisterEvent(gameEvents2, delimiter2, null, closure2, (bool?)arg4 ?? false);

            else if (arg1 is GameEvents gameEvents3 && arg2 is Table conditions3 && arg3 is Closure closure3)
                RegisterEvent(gameEvents3, string.Empty, conditions3, closure3, (bool?)arg4 ?? false);

            else if (arg1 is GameEvents gameEvents4 && arg2 is string delimiter4 && arg3 is Table conditions4 && arg4 is Closure closure4)
                RegisterEvent(gameEvents4, delimiter4, conditions4, closure4, (bool?)arg5 ?? false);

            else if (arg1 is Table gameEvents5 && arg2 is Closure closure5)
                foreach (var event5 in gameEvents5.Values)
                {
                    if (event5.Type == DataType.Function)
                    {
                        var trigger = event5.Function.Call();
                        var key = trigger.ToObject<(GameEvents, string)>().Item1;
                        var delimiter = trigger.ToObject<(GameEvents, string)>().Item2;
                        RegisterEvent(key, delimiter, null, closure5, (bool?)arg5 ?? false);
                    }
                    else
                    {
                        RegisterEvent((GameEvents)event5.Number, string.Empty, null, closure5, (bool?)arg5 ?? false);
                    }
                }

            else if (arg1 is Table gameEvents6 && arg2 is Table conditions6 && arg3 is Closure closure6)
                foreach (var event6 in gameEvents6.Values)
                {
                    if (event6.Type == DataType.Function)
                    {
                        var trigger = event6.Function.Call();
                        var key = trigger.ToObject<(GameEvents, string)>().Item1;
                        var delimiter = trigger.ToObject<(GameEvents, string)>().Item2;
                        RegisterEvent(key, delimiter, conditions6, closure6, (bool?)arg5 ?? false);
                    }
                    else
                    {
                        RegisterEvent((GameEvents)event6.Number, string.Empty, conditions6, closure6, (bool?)arg5 ?? false);
                    }
                }
        }

        private void RegisterEvent(GameEvents gameEvent, string delimiter, Table conditions, Closure closure, bool runOnce = false)
        {
            var reservedId = _events.ReserveId();

            var action = new Action<IGameEventArgs>(obj =>
            {
                // condition checks first
                if (conditions != null)
                    if (conditions.Values.Select(condition => condition.Function).Any(cs => cs.Call() == DynValue.False))
                        return;

                ScriptHost.RunCoroutine($"event subscribed to ({Enum.GetName(typeof(GameEvents), gameEvent)}, {delimiter})", closure);

                if (runOnce)
                    _events.Unsubscribe(reservedId);
            });

            _events.Subscribe(reservedId, gameEvent, delimiter, action);
        }
    }
}
