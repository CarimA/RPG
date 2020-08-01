using System.Collections;
using System.Collections.Generic;

namespace PhotoVs.Engine.StateMachine
{
    public class StackStateMachine<T> : IEnumerable<T> where T : State
    {
        public StackStateMachine()
        {
            States = new Stack<T>();
        }

        public Stack<T> States { get; }

        public IEnumerator<T> GetEnumerator()
        {
            return States.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return States.GetEnumerator();
        }

        public virtual T Peek()
        {
            return States.Count > 0 ? States.Peek() : default;
        }

        public virtual void Push(T state)
        {
            Peek()?.OnDeactivate();
            States.Push(state);
            Peek()?.OnActivate();
        }

        public virtual T Pop()
        {
            Peek()?.OnDeactivate();
            var output = States.Pop();
            Peek()?.OnActivate();
            return output;
        }

        public virtual IEnumerable<T> PopAll()
        {
            while (States.Count > 0)
                yield return Pop();
        }
    }
}