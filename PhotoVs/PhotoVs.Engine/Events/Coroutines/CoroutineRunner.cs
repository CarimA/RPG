using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PhotoVs.Engine.Events.Coroutines.Instructions;

namespace PhotoVs.Engine.Events.Coroutines
{
    public class CoroutineRunner
    {
        // Timings guide: https://www.alanzucconi.com/2017/02/15/nested-coroutines-in-unity/

        private readonly List<Coroutine> _coroutines;

        public CoroutineRunner()
        {
            _coroutines = new List<Coroutine>();
        }

        public void Start(Coroutine coroutine)
        {
            _coroutines.Add(coroutine);
        }

        public void Stop(Coroutine coroutine)
        {
            _coroutines.Remove(coroutine);
        }

        public void StopAll()
        {
            _coroutines.Clear();
        }

        public void Update(GameTime gameTime)
        {
            for (var i = 0; i < _coroutines.Count; i++)
            {
                var coroutine = _coroutines[i];

                // this coroutine has finished
                if (coroutine.Current == null)
                    if (!coroutine.MoveNext())
                        _coroutines.RemoveAt(i--);

                switch (coroutine.Current)
                {
                    case IYield instruction:
                        if (instruction.CanContinue(gameTime))
                            if (!coroutine.MoveNext())
                                _coroutines.RemoveAt(i--);
                        break;

                    case Coroutine routine:
                        if (routine.Current is IYield yieldA)
                            if (yieldA.CanContinue(gameTime))
                                if (!routine.MoveNext())
                                    if (!coroutine.MoveNext())
                                        _coroutines.RemoveAt(i--);
                        break;

                    case IEnumerator enumerator:
                        if (enumerator.Current is IYield yieldB)
                        {
                            if (yieldB.CanContinue(gameTime))
                                if (!enumerator.MoveNext())
                                    if (!coroutine.MoveNext())
                                        _coroutines.RemoveAt(i--);
                        }
                        else if (!enumerator.MoveNext())
                            if (!coroutine.MoveNext())
                                _coroutines.RemoveAt(i--);

                        break;
                    case null:
                        coroutine.MoveNext();

                        break;

                    default:
                        throw new ArgumentOutOfRangeException(coroutine.Current.GetType().ToString());
                }
            }
        }
    }
}
