﻿using Microsoft.Xna.Framework;
using PhotoVs.Engine.Scripting;
using PhotoVs.Utils.Extensions;
using System;

namespace PhotoVs.Logic.Modules
{
    public class TimingModule : Module
    {
        private GameTime _latestGameTime;

        public TimingModule()
        {
            _latestGameTime = new GameTime();
        }

        public override void DefineApi(MoonSharpInterpreter interpreter)
        {
            if (interpreter == null)
                throw new ArgumentNullException(nameof(interpreter));

            interpreter.AddFunction("GetDeltaTime", (Func<float>)GetDeltaTime);
            interpreter.AddFunction("GetTotalTime", (Func<float>)GetTime);

            base.DefineApi(interpreter);
        }

        private float GetDeltaTime()
        {
            return _latestGameTime.GetElapsedSeconds();
        }

        private float GetTime()
        {
            return _latestGameTime.GetTotalSeconds();
        }

        public override void Update(GameTime gameTime)
        {
            _latestGameTime = gameTime;
            base.Update(gameTime);
        }
    }
}
