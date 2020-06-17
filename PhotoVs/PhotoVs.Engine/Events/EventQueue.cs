using PhotoVs.Utils.Logging;
using System;
using System.Collections.Generic;

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

        public string Subscribe(string eventType, Action<IGameEventArgs> gameEvent)
        {
            var id = Guid.NewGuid().ToString();
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
            foreach (var toSubscribe in _toSubscribe)
            {
                var eventType = toSubscribe.Item1;
                var id = toSubscribe.Item2;
                var gameEvent = toSubscribe.Item3;

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
                _events.Remove(toUnsubscribe);
                _subscriptions[_eventTypeReference[toUnsubscribe]].Remove(toUnsubscribe);
                _eventTypeReference.Remove(toUnsubscribe);

                Logger.Write.Trace($"Unsubscribing ID: {toUnsubscribe}).");
            }

            _toUnsubscribe.Clear();
        }

        private void ProcessNotifications()
        {
            foreach (var notification in _notifications)
            {
                var eventType = notification.Item1;
                var gameEvent = notification.Item2;

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
