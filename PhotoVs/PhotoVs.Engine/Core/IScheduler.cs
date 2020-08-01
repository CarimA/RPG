using Microsoft.Xna.Framework;

namespace PhotoVs.Engine.Core
{
    public interface IScheduler
    {
        void Start();
        void BeforeUpdate(GameTime gameTime);
        void Update(GameTime gameTime);
        void AfterUpdate(GameTime gameTime);
        void BeforeDraw(GameTime gameTime);
        void Draw(GameTime gameTime);
        void AfterDraw(GameTime gameTime);
    }
}