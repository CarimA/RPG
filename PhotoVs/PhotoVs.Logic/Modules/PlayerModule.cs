using PhotoVs.Engine.Scripting;
using PhotoVs.Logic.PlayerData;
using System;

namespace PhotoVs.Logic.Modules
{
    public class PlayerModule : Module
    {
        private readonly Player _player;

        public PlayerModule(Player player)
        {
            _player = player;
        }

        public override void DefineApi(MoonSharpInterpreter interpreter)
        {
            if (interpreter == null)
                throw new ArgumentNullException(nameof(interpreter));

            interpreter.AddFunction("Lock", (Action)LockMovement);
            interpreter.AddFunction("Unlock", (Action)UnlockMovement);
            interpreter.AddFunction("Flag", (Func<string, bool?, bool>)Flag);
            interpreter.AddFunction("Var", (Func<string, IComparable, object>)Var);
            interpreter.AddFunction("Save", (Action)Save);

            base.DefineApi(interpreter);
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
            else
                return _player.PlayerData.GetFlag(flag);
        }

        private object Var(string flag, IComparable value = null)
        {
            if (value != null)
            {
                _player.PlayerData.SetVariable(flag, value);
                return value;
            }
            else
                return _player.PlayerData.GetVariable(flag);
        }
    }
}
