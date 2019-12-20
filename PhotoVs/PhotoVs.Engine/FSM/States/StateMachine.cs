using System.Collections.Generic;
using System.Linq;
using PhotoVs.Models.FSM;

namespace PhotoVs.Engine.FSM.States
{
    public class StateMachine<T> where T : IState
    {
        private readonly Stack<T> _states;

        public StateMachine()
        {
            _states = new Stack<T>();
        }

        public virtual void Change(T state, params object[] args)
        {
            while (_states.Count > 0)
                Pop();

            Push(state, args);
        }

        public virtual void Push(T state, params object[] args)
        {
            if (state.IsBlocking)
                Peek().Suspend();

            state.Enter(args);
            _states.Push(state);
        }

        public virtual T Pop()
        {
            var pop = _states.Pop();
            pop.Exit();

            if (pop.IsBlocking)
                Peek().Resume();

            return pop;
        }

        public virtual T Peek()
        {
            return _states.Peek();
        }

        public IEnumerable<T> CurrentStates()
        {
            return _states.Take(_states.TakeWhile(IsStateNotBlocking).Count() + 1).Reverse();
        }

        private bool IsStateBlocking(T state)
        {
            return state.IsBlocking;
        }

        private bool IsStateNotBlocking(T state)
        {
            return !state.IsBlocking;
        }
    }
}