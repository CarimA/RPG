using PhotoVs.Engine.Events.EventArgs;
using PhotoVs.Logic.Mechanics.World;

namespace PhotoVs.Logic.Events.EventArgs
{
    internal class DayEventArgs : IGameEventArgs
    {
        public DayEventArgs(object sender, Day day)
        {
            Sender = sender;
            Day = day;
        }

        public Day Day { get; }
        public object Sender { get; }
    }
}