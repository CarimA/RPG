using PhotoVs.Engine.ECS.GameObjects;

namespace PhotoVs.Engine.Events
{
    public class InteractEventArgs : IGameEventArgs
    {
        public object Sender { get; }
        public IGameObject ObjectA { get; }
        public IGameObject ObjectB { get; }

        public InteractEventArgs(object sender, IGameObject objectA, IGameObject objectB)
        {
            Sender = sender;
            ObjectA = objectA;
            ObjectB = objectB;
        }
    }
}
