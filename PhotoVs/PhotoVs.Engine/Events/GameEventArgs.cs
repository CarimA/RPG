using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
