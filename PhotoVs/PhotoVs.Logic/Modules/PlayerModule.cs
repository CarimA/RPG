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
            interpreter.AddFunction("lock", (Action)LockMovement);
            interpreter.AddFunction("unlock", (Action)UnlockMovement);

            base.DefineApi(interpreter);
        }

        private void UnlockMovement()
        {
            _player.UnlockMovement();
        }

        private void LockMovement()
        {
            _player.LockMovement();
        }
    }
}
