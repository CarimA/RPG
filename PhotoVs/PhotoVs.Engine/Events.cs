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
    public class InteractEventArgs : EventArgs
    {
        public IGameObject Player { get; set; }
        public IGameObject Script { get; set; }
    }


    public sealed class Events
    {
        public readonly IndexedEvent<string, Action<InteractEventArgs>> OnInteractEventAction;
        public readonly IndexedEvent<string, Action<InteractEventArgs>> OnInteractEventEnter;
        public readonly IndexedEvent<string, Action<InteractEventArgs>> OnInteractEventExit;
        public readonly IndexedEvent<string, Action<InteractEventArgs>> OnInteractEventRun;
        public readonly IndexedEvent<string, Action<InteractEventArgs>> OnInteractEventStand;
        public readonly IndexedEvent<string, Action<InteractEventArgs>> OnInteractEventWalk;

        public Events()
        {
            OnInteractEventAction = new IndexedEvent<string, Action<InteractEventArgs>>();
            OnInteractEventEnter = new IndexedEvent<string, Action<InteractEventArgs>>();
            OnInteractEventExit = new IndexedEvent<string, Action<InteractEventArgs>>();
            OnInteractEventStand = new IndexedEvent<string, Action<InteractEventArgs>>();
            OnInteractEventWalk = new IndexedEvent<string, Action<InteractEventArgs>>();
            OnInteractEventRun = new IndexedEvent<string, Action<InteractEventArgs>>();
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

        public Action OnGameStart;
        public Action<CollisionEventArgs> OnCollision;

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
            if (OnInteractEventAction.TryGetValue(key, out var value))
                value?.Invoke(this, player, script);
        }

        public void RaiseOnInteractEventEnter(string key, IGameObject player, IGameObject script)
        {
            Logger.Write.Trace($"EVENT - Invoking OnInteractEventEnter ({key})");
            if (OnInteractEventEnter.TryGetValue(key, out var value))
                value?.Invoke(this, player, script);
        }

        public void RaiseOnInteractEventExit(string key, IGameObject player, IGameObject script)
        {
            Logger.Write.Trace($"EVENT - Invoking OnInteractEventExit ({key})");
            if (OnInteractEventExit.TryGetValue(key, out var value))
                value?.Invoke(this, player, script);
        }

        public void RaiseOnInteractEventStand(string key, IGameObject player, IGameObject script)
        {
            Logger.Write.Trace($"EVENT - Invoking OnInteractEventStand ({key})");
            if (OnInteractEventStand.TryGetValue(key, out var value))
                value?.Invoke(this, player, script);
        }

        public void RaiseOnInteractEventWalk(string key, IGameObject player, IGameObject script)
        {
            Logger.Write.Trace($"EVENT - Invoking OnInteractEventWalk ({key})");
            if (OnInteractEventWalk.TryGetValue(key, out var value))
                value?.Invoke(this, player, script);
        }

        public void RaiseOnInteractEventRun(string key, IGameObject player, IGameObject script)
        {
            Logger.Write.Trace($"EVENT - Invoking OnInteractEventRun ({key})");
            if (OnInteractEventRun.TryGetValue(key, out var value))
                value?.Invoke(this, player, script);
        }
    }
}