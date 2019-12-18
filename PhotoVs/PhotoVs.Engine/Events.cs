using PhotoVs.Models.ECS;
using PhotoVs.Utils.Logging;

namespace PhotoVs.Engine
{
    public sealed class Events
    {
        public delegate void VoidEventHandler(object sender);
        public delegate void CollisionEventHandler(object sender, IGameObject moving, IGameObject stationary);
        public delegate void InteractEventHandler(object sender, IGameObject player, IGameObject script);

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
        public readonly IndexedEvent<InteractEventHandler> OnInteractEventAction;
        public readonly IndexedEvent<InteractEventHandler> OnInteractEventEnter;
        public readonly IndexedEvent<InteractEventHandler> OnInteractEventExit;
        public readonly IndexedEvent<InteractEventHandler> OnInteractEventStand;
        public readonly IndexedEvent<InteractEventHandler> OnInteractEventWalk;
        public readonly IndexedEvent<InteractEventHandler> OnInteractEventRun;

        public Events()
        {
            OnInteractEventAction = new IndexedEvent<InteractEventHandler>();
            OnInteractEventEnter = new IndexedEvent<InteractEventHandler>();
            OnInteractEventExit = new IndexedEvent<InteractEventHandler>();
            OnInteractEventStand = new IndexedEvent<InteractEventHandler>();
            OnInteractEventWalk = new IndexedEvent<InteractEventHandler>();
            OnInteractEventRun = new IndexedEvent<InteractEventHandler>();
        }

        public void RaiseOnGameStart()
        {
            Debug.Log.Trace("EVENT - Invoking OnGameStart");
            OnGameStart?.Invoke(this);
        }

        public void RaiseOnCollision(IGameObject moving, IGameObject stationary)
        {
            Debug.Log.Trace("EVENT - Invoking OnCollision");
            OnCollision?.Invoke(this, moving, stationary);
        }

        public void RaiseOnInteractEventAction(string key, IGameObject player, IGameObject script)
        {
            Debug.Log.Trace($"EVENT - Invoking OnInteractEventAction ({key})");
            if (OnInteractEventAction.TryGetValue(key, out var value))
            {
                value?.Invoke(this, player, script);
            }
        }
        public void RaiseOnInteractEventEnter(string key, IGameObject player, IGameObject script)
        {
            Debug.Log.Trace($"EVENT - Invoking OnInteractEventEnter ({key})");
            if (OnInteractEventEnter.TryGetValue(key, out var value))
            {
                value?.Invoke(this, player, script);
            }
        }
        public void RaiseOnInteractEventExit(string key, IGameObject player, IGameObject script)
        {
            Debug.Log.Trace($"EVENT - Invoking OnInteractEventExit ({key})");
            if (OnInteractEventExit.TryGetValue(key, out var value))
            {
                value?.Invoke(this, player, script);
            }
        }
        public void RaiseOnInteractEventStand(string key, IGameObject player, IGameObject script)
        {
            Debug.Log.Trace($"EVENT - Invoking OnInteractEventStand ({key})");
            if (OnInteractEventStand.TryGetValue(key, out var value))
            {
                value?.Invoke(this, player, script);
            }
        }
        public void RaiseOnInteractEventWalk(string key, IGameObject player, IGameObject script)
        {
            Debug.Log.Trace($"EVENT - Invoking OnInteractEventWalk ({key})");
            if (OnInteractEventWalk.TryGetValue(key, out var value))
            {
                value?.Invoke(this, player, script);
            }
        }
        public void RaiseOnInteractEventRun(string key, IGameObject player, IGameObject script)
        {
            Debug.Log.Trace($"EVENT - Invoking OnInteractEventRun ({key})");
            if (OnInteractEventRun.TryGetValue(key, out var value))
            {
                value?.Invoke(this, player, script);
            }
        }
    }
}