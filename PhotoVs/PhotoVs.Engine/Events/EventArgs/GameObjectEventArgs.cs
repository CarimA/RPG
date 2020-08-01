using PhotoVs.Engine.ECS;

namespace PhotoVs.Engine.Events.EventArgs
{
    public class GameObjectEventArgs : IGameEventArgs
    {
        public GameObjectEventArgs(object sender, GameObject obj)
        {
            Sender = sender;
            GameObject = obj;
        }

        public GameObject GameObject { get; }
        public object Sender { get; }
    }
}