namespace PhotoVs.Engine.StateMachine
{
    public abstract class State
    {
        // called when a state is transitioned into
        public virtual void OnActivate()
        {
        }

        // called when a state is transitioned out
        public virtual void OnDeactivate()
        {
        }
    }
}
