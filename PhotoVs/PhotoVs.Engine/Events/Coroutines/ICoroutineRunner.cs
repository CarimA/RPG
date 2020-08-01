using Microsoft.Xna.Framework;

namespace PhotoVs.Engine.Events.Coroutines
{
    public interface ICoroutineRunner
    {
        void Start(Coroutine coroutine);
        void Stop(Coroutine coroutine);
        void StopAll();
        void Update(GameTime gameTime);
    }
}