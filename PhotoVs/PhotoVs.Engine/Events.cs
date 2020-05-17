using PhotoVs.Engine.ECS.GameObjects;
using PhotoVs.Utils.Logging;
using System;

namespace PhotoVs.Engine
{
    public sealed class Events
    {
        public readonly IndexedEvent<string, Action<IGameObject, IGameObject>> OnInteractEventAction;
        public readonly IndexedEvent<string, Action<IGameObject, IGameObject>> OnInteractEventEnter;
        public readonly IndexedEvent<string, Action<IGameObject, IGameObject>> OnInteractEventExit;
        public readonly IndexedEvent<string, Action<IGameObject, IGameObject>> OnInteractEventRun;
        public readonly IndexedEvent<string, Action<IGameObject, IGameObject>> OnInteractEventStand;
        public readonly IndexedEvent<string, Action<IGameObject, IGameObject>> OnInteractEventWalk;

        public Events()
        {
            OnInteractEventAction = new IndexedEvent<string, Action<IGameObject, IGameObject>>();
            OnInteractEventEnter = new IndexedEvent<string, Action<IGameObject, IGameObject>>();
            OnInteractEventExit = new IndexedEvent<string, Action<IGameObject, IGameObject>>();
            OnInteractEventStand = new IndexedEvent<string, Action<IGameObject, IGameObject>>();
            OnInteractEventWalk = new IndexedEvent<string, Action<IGameObject, IGameObject>>();
            OnInteractEventRun = new IndexedEvent<string, Action<IGameObject, IGameObject>>();
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
        public Action<IGameObject, IGameObject> OnCollision;

        public void RaiseOnGameStart()
        {
            Logger.Write.Trace("EVENT - Invoking OnGameStart");
            OnGameStart?.Invoke();
        }

        public void RaiseOnCollision(IGameObject moving, IGameObject stationary)
        {
            Logger.Write.Trace("EVENT - Invoking OnCollision");
            OnCollision?.Invoke(moving, stationary);
        }

        public void RaiseOnInteractEventAction(string key, IGameObject player, IGameObject script)
        {
            Logger.Write.Trace($"EVENT - Invoking OnInteractEventAction ({key})");
            if (OnInteractEventAction.TryGetValue(key, out var value))
                value?.Invoke(player, script);
        }

        public void RaiseOnInteractEventEnter(string key, IGameObject player, IGameObject script)
        {
            Logger.Write.Trace($"EVENT - Invoking OnInteractEventEnter ({key})");
            if (OnInteractEventEnter.TryGetValue(key, out var value))
                value?.Invoke(player, script);
        }

        public void RaiseOnInteractEventExit(string key, IGameObject player, IGameObject script)
        {
            Logger.Write.Trace($"EVENT - Invoking OnInteractEventExit ({key})");
            if (OnInteractEventExit.TryGetValue(key, out var value))
                value?.Invoke(player, script);
        }

        public void RaiseOnInteractEventStand(string key, IGameObject player, IGameObject script)
        {
            Logger.Write.Trace($"EVENT - Invoking OnInteractEventStand ({key})");
            if (OnInteractEventStand.TryGetValue(key, out var value))
                value?.Invoke(player, script);
        }

        public void RaiseOnInteractEventWalk(string key, IGameObject player, IGameObject script)
        {
            Logger.Write.Trace($"EVENT - Invoking OnInteractEventWalk ({key})");
            if (OnInteractEventWalk.TryGetValue(key, out var value))
                value?.Invoke(player, script);
        }

        public void RaiseOnInteractEventRun(string key, IGameObject player, IGameObject script)
        {
            Logger.Write.Trace($"EVENT - Invoking OnInteractEventRun ({key})");
            if (OnInteractEventRun.TryGetValue(key, out var value))
                value?.Invoke(player, script);
        }
    }
}