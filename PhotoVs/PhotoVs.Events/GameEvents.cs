using System;
using System.Collections.Generic;

namespace PhotoVs.Events
{
    public class GameEvents
    {
        private readonly Dictionary<string, Action<GameEvents, IGameEventArgs>> _events;

        public GameEvents()
        {
            _events = new Dictionary<string, Action<GameEvents, IGameEventArgs>>();
        }

        public Action<GameEvents, IGameEventArgs> this[string id]
        {
            get
            {
                if (!_events.ContainsKey(id)) _events.Add(id, null);

                return _events[id];
            }
            set
            {
                if (!_events.ContainsKey(id))
                    _events.Add(id, null);

                _events[id] = value;
            }
        }

        public void Raise(string id, IGameEventArgs args = null)
        {
            if (_events.TryGetValue(id, out var gameEvent))
                // todo: Log each event fired
                gameEvent(this, args);
        }

        public void Clear(string id)
        {
            _events.Remove(id);
        }
    }
}