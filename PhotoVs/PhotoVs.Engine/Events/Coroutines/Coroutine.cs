using System.Collections;

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
