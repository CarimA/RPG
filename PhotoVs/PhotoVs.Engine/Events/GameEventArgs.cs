namespace PhotoVs.Engine.Events
{
    public class GameEventArgs : IGameEventArgs
    {
        public object Sender { get; }

        public GameEventArgs(object sender)
        {
            Sender = sender;
        }
    }
}
