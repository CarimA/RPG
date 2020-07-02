using MoonSharp.Interpreter;
using PhotoVs.Engine.Scripting;
using PhotoVs.Logic.PlayerData;
using System;
using System.ComponentModel;

namespace PhotoVs.Logic.Modules
{
    public class EventConditionsModule : Module
    {
        private readonly Player _player;

        private enum Equality
        {
            Equals = 0,
            GreaterThan,
            GreaterThanOrEquals,
            LessThan,
            LessThanOrEquals,
            NotEquals
        }

        public EventConditionsModule(Player player)
        {
            _player = player;
        }

        public override void DefineApi(MoonSharpInterpreter interpreter)
        {
            if (interpreter == null)
                throw new ArgumentNullException(nameof(interpreter));

            interpreter.AddFunction("_Flag", (Func<string, bool, bool>)CheckFlag);
            interpreter.AddFunction("_Var", (Func<string, Equality, object, bool>)CheckVariable);
            interpreter.AddType<Equality>("Equality");

            base.DefineApi(interpreter);
        }

        private bool CheckVariable(string variable, Equality equality, object obj)
        {
            var playerVar = _player.PlayerData.GetVariable(variable);
            return equality switch
            {
                Equality.Equals => !(playerVar.Equals(variable)),
                Equality.GreaterThan => !(playerVar.CompareTo(variable) > 0),
                Equality.GreaterThanOrEquals => !((playerVar.CompareTo(variable) > 0) &&
                                                  playerVar.Equals(variable)),
                Equality.LessThan => !(playerVar.CompareTo(variable) < 0),
                Equality.LessThanOrEquals => !((playerVar.CompareTo(variable) < 0) &&
                                               playerVar.Equals(variable)),
                Equality.NotEquals => playerVar.Equals(variable),
                _ => throw new InvalidEnumArgumentException(nameof(equality)),
            };
        }

        private bool CheckFlag(string flag, bool state)
        {
            return _player.PlayerData.GetFlag(flag) == state;
        }
    }
}
