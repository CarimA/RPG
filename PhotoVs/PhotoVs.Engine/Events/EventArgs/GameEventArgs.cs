namespace PhotoVs.Engine.Events.EventArgs
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
