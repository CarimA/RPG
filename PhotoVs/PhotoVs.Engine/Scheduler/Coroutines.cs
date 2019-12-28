using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PhotoVs.Engine.Scheduler.YieldInstructions;

namespace PhotoVs.Engine.Scheduler
{
    public class Coroutines
    {
        // Timings guide: https://www.alanzucconi.com/2017/02/15/nested-coroutines-in-unity/
        private readonly List<IEnumerator> _routines;

        public Coroutines()
        {
            _routines = new List<IEnumerator>();
        }

        public void Start(IEnumerator routine)
        {
            _routines.Add(routine);
        }

        public void Stop(IEnumerator routine)
        {
            _routines.Remove(routine);
        }

        public void StopAll()
        {
            _routines.Clear();
        }

        public void Update(GameTime gameTime)
        {
            for (var i = 0; i < _routines.Count; i++)
            {
                var routine = _routines[i];

                if (routine.Current == null)
                    if (!routine.MoveNext())
                        _routines.RemoveAt(i--);
                // this routine has finished

                if (routine.Current is IEnumerator enumerator)
                {
                    if (enumerator.Current is IYieldInstruction instruction)
                        if (instruction.Continue(gameTime))
                            if (!enumerator.MoveNext())
                                if (!routine.MoveNext())
                                    _routines.RemoveAt(i--);
                }
                else if (routine.Current is IYieldInstruction instruction)
                {
                    if (instruction.Continue(gameTime))
                        if (!routine.MoveNext())
                            _routines.RemoveAt(i--);
                }
            }
        }
    }
}