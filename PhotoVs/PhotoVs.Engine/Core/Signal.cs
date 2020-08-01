using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PhotoVs.Engine.Events.EventArgs;
using PhotoVs.Utils.Logging;

namespace PhotoVs.Engine.Core
{
    public class Signal : ISignal, IHasBeforeUpdate
    {
        // a dictionary holding every subscription with its ID
        private readonly Dictionary<string, Action<IGameEventArgs>> _methods;

        // a dictionary holding the event type the ID is associated with
        private readonly Dictionary<string, string> _references;

        // a dictionary holding a list of IDs that match with an event 
        private readonly Dictionary<string, List<string>> _subscriptions;
        private readonly List<(string, IGameEventArgs)> _toNotify;

        private readonly List<(string, string, Action<IGameEventArgs>)> _toSubscribe;
        private readonly List<string> _toUnsubscribe;

        public Signal()
        {
            _subscriptions = new Dictionary<string, List<string>>();
            _methods = new Dictionary<string, Action<IGameEventArgs>>();
            _references = new Dictionary<string, string>();
            _toSubscribe = new List<(string, string, Action<IGameEventArgs>)>();
            _toUnsubscribe = new List<string>();
            _toNotify = new List<(string, IGameEventArgs)>();
        }

        public int BeforeUpdatePriority { get; set; } = 0;
        public bool BeforeUpdateEnabled { get; set; } = true;

        public void BeforeUpdate(GameTime gameTime)
        {
            ProcessSubscriptions();
            ProcessUnsubscriptions();
            ProcessNotifications();
        }

        public string ReserveId()
        {
            return Guid.NewGuid().ToString();
        }

        public void Subscribe(string id, string signal, Action<IGameEventArgs> action)
        {
            _toSubscribe.Add((signal, id, action));
        }

        public string Subscribe(string signal, Action<IGameEventArgs> action)
        {
            var id = ReserveId();
            Subscribe(id, signal, action);
            return id;
        }

        public bool Unsubscribe(string id)
        {
            if (!_methods.ContainsKey(id))
                return false;
            _toUnsubscribe.Add(id);
            return true;
        }

        public void Notify(string signal, IGameEventArgs args)
        {
            _toNotify.Add((signal, args));
            if (signal.Contains(":"))
            {
                var first = signal.Split(':')[0];
                Notify(first, args);
            }
        }

        private void ProcessSubscriptions()
        {
            foreach (var (signal, id, gameEvent) in _toSubscribe)
            {
                _methods.Add(id, gameEvent);
                _references.Add(id, signal);
                if (!_subscriptions.ContainsKey(signal))
                    _subscriptions.Add(signal, new List<string>());
                _subscriptions[signal].Add(id);

                Logger.Write.Trace($"Subscribing to event \"{signal}\" (ID: {id}).");
            }

            _toSubscribe.Clear();
        }

        private void ProcessUnsubscriptions()
        {
            foreach (var toUnsubscribe in _toUnsubscribe)
            {
                var eventId = _references[toUnsubscribe];
                _methods.Remove(toUnsubscribe);
                _subscriptions[eventId].Remove(toUnsubscribe);
                _references.Remove(toUnsubscribe);

                if (_subscriptions[eventId].Count == 0)
                    _subscriptions.Remove(eventId);

                Logger.Write.Trace($"Unsubscribing ID: {toUnsubscribe}).");
            }

            _toUnsubscribe.Clear();
        }

        private void ProcessNotifications()
        {
            foreach (var (eventType, gameEvent) in _toNotify)
            {
                Logger.Write.Trace($"Processing event \"{eventType}\".");

                if (!_subscriptions.TryGetValue(eventType, out var subscriptions))
                    continue;

                if (_subscriptions.Count == 0)
                {
                    continue;
                }

                Logger.Write.Trace($"Notifying event \"{eventType}\" to {subscriptions.Count} listeners.");

                foreach (var sub in subscriptions)
                {
                    _methods[sub](gameEvent);
                }
            }

            _toNotify.Clear();
        }
    }
}