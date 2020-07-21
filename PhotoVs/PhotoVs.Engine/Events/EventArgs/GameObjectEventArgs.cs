using PhotoVs.Engine.ECS;

namespace PhotoVs.Engine.Events.EventArgs
{
    public class GameObjectEventArgs : IGameEventArgs
    {
        public object Sender { get; }
        public GameObject GameObject { get; }

        public GameObjectEventArgs(object sender, GameObject obj)
        {
            Sender = sender;
            GameObject = obj;
        }
    }
}
