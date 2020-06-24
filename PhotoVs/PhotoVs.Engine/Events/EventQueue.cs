using System;
using System.Collections.Generic;
using PhotoVs.Engine.Events.EventArgs;
using PhotoVs.Utils.Logging;

public class EventQueue<T> where T : Enum
{
    // a dictionary holding a list of IDs that match with an event 
    private readonly Dictionary<(T, string), List<string>> _subscriptions;

    // a dictionary holding every subscription with its ID
    private readonly Dictionary<string, Action<IGameEventArgs>> _methods;

    // a dictionary holding the event type the ID is associated with
    private readonly Dictionary<string, (T, string)> _references;

    private readonly List<((T, string), string, Action<IGameEventArgs>)> _toSubscribe;
    private readonly List<string> _toUnsubscribe;
    private readonly List<((T, string), IGameEventArgs)> _toNotify;


    public EventQueue()
    {
        _subscriptions = new Dictionary<(T, string), List<string>>();
        _methods = new Dictionary<string, Action<IGameEventArgs>>();
        _references = new Dictionary<string, (T, string)>();
        _toSubscribe = new List<((T, string), string, Action<IGameEventArgs>)>();
        _toUnsubscribe = new List<string>();
        _toNotify = new List<((T, string), IGameEventArgs)>();
    }

    public string ReserveId()
    {
        return Guid.NewGuid().ToString();
    }

    public string Subscribe(string id, T eventKey, string eventDelimiter, Action<IGameEventArgs> action)
    {
        _toSubscribe.Add(((eventKey, eventDelimiter), id, action));
        return id;
    }

    public string Subscribe(string id, T eventKey, Action<IGameEventArgs> action)
    {
        return Subscribe(id, eventKey, string.Empty, action);
    }

    public string Subscribe(T eventKey, string eventDelimiter, Action<IGameEventArgs> action)
    {
        var id = ReserveId();
        return Subscribe(id, eventKey, eventDelimiter, action);
    }

    public string Subscribe(T eventKey, Action<IGameEventArgs> action)
    {
        var id = ReserveId();
        return Subscribe(id, eventKey, action);
    }


    public bool Unsubscribe(string id)
    {
        if (!_methods.ContainsKey(id))
            return false;
        _toUnsubscribe.Add(id);
        return true;
    }

    public void Notify(T eventKey, string eventDelimiter, IGameEventArgs args)
    {
        _toNotify.Add(((eventKey, eventDelimiter), args));
        if (!eventDelimiter.Equals(string.Empty))
            _toNotify.Add(((eventKey, string.Empty), args));
    }

    public void Notify(T eventKey, IGameEventArgs args)
    {
        Notify(eventKey, string.Empty, args);
    }

    public void ProcessQueue()
    {
        ProcessSubscriptions();
        ProcessUnsubscriptions();
        ProcessNotifications();
    }

    private void ProcessSubscriptions()
    {
        foreach (var (eventType, id, gameEvent) in _toSubscribe)
        {
            _methods.Add(id, gameEvent);
            _references.Add(id, eventType);
            if (!_subscriptions.ContainsKey(eventType))
                _subscriptions.Add(eventType, new List<string>());
            _subscriptions[eventType].Add(id);

            var name = Enum.GetName(typeof(T), eventType.Item1);
            var delimiter = eventType.Item2;
            Logger.Write.Trace($"Subscribing to event \"{name}{(eventType.Item2.Equals(string.Empty) ? "" : ":" )}{delimiter}\" (ID: {id}).");
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
            if (!_subscriptions.TryGetValue(eventType, out var subscriptions))
                continue;

            if (_subscriptions.Count == 0)
                continue;

            Logger.Write.Trace($"Notifying event \"{eventType}\" with type \"{gameEvent.GetType().Name}\" to {subscriptions.Count} listeners.");

            foreach (var sub in subscriptions)
            {
                _methods[sub](gameEvent);
            }
        }

        _toNotify.Clear();
    }
}