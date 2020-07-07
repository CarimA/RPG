using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoVs.Engine.Events.EventArgs;
using PhotoVs.Logic.Mechanics.World;

namespace PhotoVs.Logic.Events.EventArgs
{
    class TimeEventArgs : IGameEventArgs
    {
        public object Sender { get; }
        public TimePhase TimePhase { get; }

        public TimeEventArgs(object sender, TimePhase timePhase)
        {
            Sender = sender;
            TimePhase = timePhase;
        }
    }
}
