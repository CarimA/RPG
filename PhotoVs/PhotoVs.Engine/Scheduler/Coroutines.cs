using Microsoft.Xna.Framework;
using PhotoVs.Engine.Scheduler.YieldInstructions;
using System.Collections;
using System.Collections.Generic;

namespace PhotoVs.Engine.Scheduler
{
    public class Coroutines
    {
        private List<IEnumerator> _routines;

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
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            for (var i = 0; i < _routines.Count; i++)
            {
                var routine = _routines[i];

                if (routine.Current == null)
                {
                    if (!routine.MoveNext())
                    {
                        // this routine has finished
                        _routines.RemoveAt(i--);
                    }
                }

                if (routine.Current is IYieldInstruction instruction)
                {
                    if (instruction.Continue(gameTime))
                    {
                        routine.MoveNext();
                    }

                    continue;
                }
            }
        }
    }
}
