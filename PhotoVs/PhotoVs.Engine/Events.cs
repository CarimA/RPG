using System;
using PhotoVs.Models.ECS;
using PhotoVs.Utils.Logging;

namespace PhotoVs.Engine
{
    public class CollisionEventArgs : EventArgs
    {
        public IGameObject Moving { get; set; }
        public IGameObject Stationary { get; set; }
    }

    public sealed class Events
    {
        public delegate void CollisionEventHandler(object sender, CollisionEventArgs e);

        public delegate void InteractEventHandler(object sender, IGameObject player, IGameObject script);

        public delegate void ServiceSetEventHandler(object sender, object service);

        public delegate void VoidEventHandler(object sender);

        public readonly IndexedEvent<string, InteractEventHandler> OnInteractEventAction;
        public readonly IndexedEvent<string, InteractEventHandler> OnInteractEventEnter;
        public readonly IndexedEvent<string, InteractEventHandler> OnInteractEventExit;
        public readonly IndexedEvent<string, InteractEventHandler> OnInteractEventRun;
        public readonly IndexedEvent<string, InteractEventHandler> OnInteractEventStand;
        public readonly IndexedEvent<string, InteractEventHandler> OnInteractEventWalk;
        public readonly IndexedEvent<Type, ServiceSetEventHandler> OnServiceSet;

        public Events()
        {
            OnServiceSet = new IndexedEvent<Type, ServiceSetEventHandler>();
            OnInteractEventAction = new IndexedEvent<string, InteractEventHandler>();
            OnInteractEventEnter = new IndexedEvent<string, InteractEventHandler>();
            OnInteractEventExit = new IndexedEvent<string, InteractEventHandler>();
            OnInteractEventStand = new IndexedEvent<string, InteractEventHandler>();
            OnInteractEventWalk = new IndexedEvent<string, InteractEventHandler>();
            OnInteractEventRun = new IndexedEvent<string, InteractEventHandler>();
        }

        // events which would be nice
        /*
         * OnSaveLoaded
         * OnSaveCreated
         * OnStep
         * OnWalkStart
         * OnWalkEnd
         * OnRunStart
         * OnRunEnd
         * OnStandStart
         * OnStandEnd
         */

        public event VoidEventHandler OnGameStart;
        public event CollisionEventHandler OnCollision;

        public void RaiseOnServiceSet<T>(T service)
        {
            Logger.Write.Trace($"EVENT - Invoking OnServiceSet ({typeof(T).Name})");
            if (OnServiceSet.TryGetValue(typeof(T), out var value)) value?.Invoke(this, service);
        }

        public void RaiseOnGameStart()
        {
            Logger.Write.Trace("EVENT - Invoking OnGameStart");
            OnGameStart?.Invoke(this);
        }

        public void RaiseOnCollision(CollisionEventArgs e)
        {
            Logger.Write.Trace("EVENT - Invoking OnCollision");
            OnCollision?.Invoke(this, e);
        }

        public void RaiseOnInteractEventAction(string key, IGameObject player, IGameObject script)
        {
            Logger.Write.Trace($"EVENT - Invoking OnInteractEventAction ({key})");
            if (OnInteractEventAction.TryGetValue(key, out var value)) value?.Invoke(this, player, script);
        }

        public void RaiseOnInteractEventEnter(string key, IGameObject player, IGameObject script)
        {
            Logger.Write.Trace($"EVENT - Invoking OnInteractEventEnter ({key})");
            if (OnInteractEventEnter.TryGetValue(key, out var value)) value?.Invoke(this, player, script);
        }

        public void RaiseOnInteractEventExit(string key, IGameObject player, IGameObject script)
        {
            Logger.Write.Trace($"EVENT - Invoking OnInteractEventExit ({key})");
            if (OnInteractEventExit.TryGetValue(key, out var value)) value?.Invoke(this, player, script);
        }

        public void RaiseOnInteractEventStand(string key, IGameObject player, IGameObject script)
        {
            Logger.Write.Trace($"EVENT - Invoking OnInteractEventStand ({key})");
            if (OnInteractEventStand.TryGetValue(key, out var value)) value?.Invoke(this, player, script);
        }

        public void RaiseOnInteractEventWalk(string key, IGameObject player, IGameObject script)
        {
            Logger.Write.Trace($"EVENT - Invoking OnInteractEventWalk ({key})");
            if (OnInteractEventWalk.TryGetValue(key, out var value)) value?.Invoke(this, player, script);
        }

        public void RaiseOnInteractEventRun(string key, IGameObject player, IGameObject script)
        {
            Logger.Write.Trace($"EVENT - Invoking OnInteractEventRun ({key})");
            if (OnInteractEventRun.TryGetValue(key, out var value)) value?.Invoke(this, player, script);
        }
    }
}