using PhotoVs.Utils.Logging;
using System;
using System.Collections.Generic;
using PhotoVs.Engine.Events.EventArgs;

namespace PhotoVs.Engine.Events
{
    public class EventQueue
    {
        private readonly Dictionary<string, List<string>> _subscriptions;
        private readonly Dictionary<string, Action<IGameEventArgs>> _events;
        private readonly Dictionary<string, string> _eventTypeReference;

        private readonly List<(string, string, Action<IGameEventArgs>)> _toSubscribe;
        private readonly List<string> _toUnsubscribe;
        private readonly List<(string, IGameEventArgs)> _notifications;

        public EventQueue()
        {
            _subscriptions = new Dictionary<string, List<string>>();
            _events = new Dictionary<string, Action<IGameEventArgs>>();
            _eventTypeReference = new Dictionary<string, string>();

            _toSubscribe = new List<(string, string, Action<IGameEventArgs>)>();
            _toUnsubscribe = new List<string>();
            _notifications = new List<(string, IGameEventArgs)>();
        }

        public string Reserve()
        {
            var id = Guid.NewGuid().ToString();
            return id;
        }
        public string Subscribe(string eventId, string eventType, Action<IGameEventArgs> gameEvent)
        {
            _toSubscribe.Add((eventType, eventId, gameEvent));
            return eventId;
        }

        public string Subscribe(string eventType, Action<IGameEventArgs> gameEvent)
        {
            var id = Reserve();
            _toSubscribe.Add((eventType, id, gameEvent));
            return id;
        }

        public bool Unsubscribe(string id)
        {
            if (!_events.ContainsKey(id))
                return false;
            _toUnsubscribe.Add(id);
            return true;

        }
        public void Notify(string eventType, IGameEventArgs gameEvent)
        {
            _notifications.Add((eventType, gameEvent));
        }

        public void Process()
        {
            ProcessSubscriptions();
            ProcessUnsubscriptions();
            ProcessNotifications();
        }

        private void ProcessSubscriptions()
        {
            foreach (var (eventType, id, gameEvent) in _toSubscribe)
            {
                _events.Add(id, gameEvent);
                _eventTypeReference.Add(id, eventType);
                if (!_subscriptions.ContainsKey(eventType))
                    _subscriptions.Add(eventType, new List<string>());
                _subscriptions[eventType].Add(id);

                Logger.Write.Trace($"Subscribing to event \"{eventType}\" (ID: {id}).");
            }

            _toSubscribe.Clear();
        }

        private void ProcessUnsubscriptions()
        {
            foreach (var toUnsubscribe in _toUnsubscribe)
            {
                var eventId = _eventTypeReference[toUnsubscribe];
                _events.Remove(toUnsubscribe);
                _subscriptions[eventId].Remove(toUnsubscribe);
                _eventTypeReference.Remove(toUnsubscribe);

                if (_subscriptions[eventId].Count == 0)
                    _subscriptions.Remove(eventId);

                Logger.Write.Trace($"Unsubscribing ID: {toUnsubscribe}).");
            }

            _toUnsubscribe.Clear();
        }

        private void ProcessNotifications()
        {
            foreach (var (eventType, gameEvent) in _notifications)
            {
                if (!_subscriptions.TryGetValue(eventType, out var subscriptions))
                    continue;

                if (_subscriptions.Count == 0)
                    continue;

                Logger.Write.Trace($"Notifying event \"{eventType}\" with type \"{gameEvent.GetType().Name}\" to {subscriptions.Count} listeners.");

                foreach (var sub in subscriptions)
                {
                    _events[sub](gameEvent);
                }
            }

            _notifications.Clear();
        }
    }
}
