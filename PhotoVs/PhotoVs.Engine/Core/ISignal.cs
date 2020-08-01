using System;
using PhotoVs.Engine.Events.EventArgs;

namespace PhotoVs.Engine.Core
{
    public interface ISignal
    {
        string ReserveId();
        void Subscribe(string id, string signal, Action<IGameEventArgs> action);
        string Subscribe(string signal, Action<IGameEventArgs> action);
        bool Unsubscribe(string id);
        void Notify(string signal, IGameEventArgs args);
    }
}