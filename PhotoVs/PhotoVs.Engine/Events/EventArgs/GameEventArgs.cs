namespace PhotoVs.Engine.Events.EventArgs
{
    public class GameEventArgs : IGameEventArgs
    {
        public GameEventArgs(object sender)
        {
            Sender = sender;
        }

        public object Sender { get; }
    }
}