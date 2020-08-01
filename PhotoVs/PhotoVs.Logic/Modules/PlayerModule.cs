using System;
using MoonSharp.Interpreter;
using PhotoVs.Engine.Scripting;
using PhotoVs.Logic.PlayerData;

namespace PhotoVs.Logic.Modules
{
    public class PlayerModule
    {
        private readonly Player _player;

        public PlayerModule(IInterpreter<Closure> interpreter, IGameState gameState)
        {
            _player = gameState.Player;

            interpreter.AddFunction("Lock", (Action) LockMovement);
            interpreter.AddFunction("Unlock", (Action) UnlockMovement);
            interpreter.AddFunction("Flag", (Func<string, bool?, bool>) Flag);
            interpreter.AddFunction("Var", (Func<string, IComparable, object>) Var);
            interpreter.AddFunction("Save", (Action) Save);
        }

        private void Save()
        {
            _player.PlayerData.Save();
        }

        private void UnlockMovement()
        {
            _player.UnlockMovement();
        }

        private void LockMovement()
        {
            _player.LockMovement();
        }

        private bool Flag(string flag, bool? value = null)
        {
            if (value.HasValue)
            {
                _player.PlayerData.SetFlag(flag, value.Value);
                return value.Value;
            }

            return _player.PlayerData.GetFlag(flag);
        }

        private object Var(string flag, IComparable value = null)
        {
            if (value != null)
            {
                _player.PlayerData.SetVariable(flag, value);
                return value;
            }

            return _player.PlayerData.GetVariable(flag);
        }
    }
}