using System;
using System.ComponentModel;
using MoonSharp.Interpreter;
using PhotoVs.Engine.Scripting;
using PhotoVs.Logic.PlayerData;

namespace PhotoVs.Logic.Modules
{
    public class EventConditionsModule
    {
        private readonly Player _player;

        public EventConditionsModule(IInterpreter<Closure> interpreter, GameState gameState)
        {
            _player = gameState.Player;

            interpreter.AddFunction("_FlagCondition", (Func<string, bool, bool>) CheckFlag);
            interpreter.AddFunction("_VarCondition", (Func<string, Equality, object, bool>) CheckVariable);
            interpreter.AddType<Equality>("Equality");
        }

        private bool CheckVariable(string variable, Equality equality, object obj)
        {
            var playerVar = _player.PlayerData.GetVariable(variable);
            return equality switch
            {
                Equality.Equals => !playerVar.Equals(variable),
                Equality.GreaterThan => !(playerVar.CompareTo(variable) > 0),
                Equality.GreaterThanOrEquals => !(playerVar.CompareTo(variable) > 0 &&
                                                  playerVar.Equals(variable)),
                Equality.LessThan => !(playerVar.CompareTo(variable) < 0),
                Equality.LessThanOrEquals => !(playerVar.CompareTo(variable) < 0 &&
                                               playerVar.Equals(variable)),
                Equality.NotEquals => playerVar.Equals(variable),
                _ => throw new InvalidEnumArgumentException(nameof(equality))
            };
        }

        private bool CheckFlag(string flag, bool state)
        {
            return _player.PlayerData.GetFlag(flag) == state;
        }

        private enum Equality
        {
            Equals = 0,
            GreaterThan,
            GreaterThanOrEquals,
            LessThan,
            LessThanOrEquals,
            NotEquals
        }
    }
}