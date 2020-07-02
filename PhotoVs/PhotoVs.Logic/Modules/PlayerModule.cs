﻿using PhotoVs.Engine.Scripting;
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
