using System;
using MoonSharp.Interpreter;
using PhotoVs.Engine.Scripting;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Logic.Modules
{
    public class TimingModule
    {
        private readonly IGameState _gameState;

        public TimingModule(IInterpreter<Closure> interpreter, IGameState gameState)
        {
            _gameState = gameState;

            interpreter.AddFunction("GetDeltaTime", (Func<float>) GetDeltaTime);
            interpreter.AddFunction("GetTotalTime", (Func<float>) GetTime);
        }

        private float GetDeltaTime()
        {
            return _gameState.GameTime.GetElapsedSeconds();
        }

        private float GetTime()
        {
            return _gameState.GameTime.GetTotalSeconds();
        }
    }
}