using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoVs.Engine.Events.Coroutines
{
    public class Coroutine
    {
        private IEnumerator _routine;

        public object Current => _routine.Current;

        public Coroutine(IEnumerator routine)
        {
            _routine = routine;
        }

        public bool MoveNext()
        {
            return _routine.MoveNext();
        }
    }
}
