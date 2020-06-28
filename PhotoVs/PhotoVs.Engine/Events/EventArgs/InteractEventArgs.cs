using PhotoVs.Engine.ECS;

namespace PhotoVs.Engine.Events.EventArgs
{
    public class InteractEventArgs : IGameEventArgs
    {
        public object Sender { get; }
        public GameObject ObjectA { get; }
        public GameObject ObjectB { get; }

        public InteractEventArgs(object sender, GameObject objectA, GameObject objectB)
        {
            Sender = sender;
            ObjectA = objectA;
            ObjectB = objectB;
        }
    }
}
