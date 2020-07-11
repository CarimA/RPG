using System.Collections;
using System.Collections.Generic;

namespace PhotoVs.Engine.StateMachine
{
    public class StackStateMachine<T> : IEnumerable<T> where T : State
    {
        private readonly Stack<T> _states;
        public Stack<T> States => _states;

        public StackStateMachine()
        {
            _states = new Stack<T>();
        }

        public virtual T Peek()
        {
            return _states.Count > 0 ? _states.Peek() : default;
        }

        public virtual void Push(T state)
        {
            Peek()?.OnDeactivate();
            _states.Push(state);
            Peek()?.OnActivate();
        }

        public virtual T Pop()
        {
            Peek()?.OnDeactivate();
            var output = _states.Pop();
            Peek()?.OnActivate();
            return output;
        }

        public virtual IEnumerable<T> PopAll()
        {
            while (_states.Count > 0)
                yield return Pop();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _states.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _states.GetEnumerator();
        }
    }
}
