using System;
using System.Collections;
using PhotoVs.Engine.Scripting;

namespace PhotoVs.Engine.Events.Coroutines
{
    public class Coroutine
    {
        public string Source { get; }
        private IEnumerator _routine;

        public object Current => _routine.Current;

        public Coroutine(IEnumerator routine)
        {
            Source = "anonymous";
            _routine = routine;
        }

        public Coroutine(string source, IEnumerator routine)
        {
            Source = source;
            _routine = routine;
        }

        public bool MoveNext()
        {
            try
            {
                return _routine.MoveNext();
            }
            catch (Exception e)
            {
                MoonSharpInterpreter.HandleError(e, Source);
            }

            return false;
        }
    }
}
