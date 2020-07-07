using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoVs.Engine.Events.EventArgs;
using PhotoVs.Logic.Mechanics.World;

namespace PhotoVs.Logic.Events.EventArgs
{
    class DayEventArgs : IGameEventArgs
    {
        public object Sender { get; }
        public Day Day { get; }

        public DayEventArgs(object sender, Day day)
        {
            Sender = sender;
            Day = day;
        }
    }
}
