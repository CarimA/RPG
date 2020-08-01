using PhotoVs.Engine.Events.EventArgs;
using PhotoVs.Logic.Mechanics.World;

namespace PhotoVs.Logic.Events.EventArgs
{
    internal class TimeEventArgs : IGameEventArgs
    {
        public TimeEventArgs(object sender, TimePhase timePhase)
        {
            Sender = sender;
            TimePhase = timePhase;
        }

        public TimePhase TimePhase { get; }
        public object Sender { get; }
    }
}