using Microsoft.Xna.Framework;
using PhotoVs.Engine.ECS;
using PhotoVs.Engine.StateMachine;
using PhotoVs.Logic.Mechanics.Input.Components;

namespace PhotoVs.Logic.NewScenes
{
    public abstract class Scene : State
    {
        public Scene()
        {
            Systems = new SystemList();
        }

        public Scene(SystemList systems)
        {
            Systems = systems;
        }

        public virtual bool IsBlocking { get; }
        public virtual SystemList Systems { get; }

        public virtual void ProcessInput(GameTime gameTime, CInputState inputState)
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Draw(GameTime gameTime, Matrix uiOrigin)
        {
        }
    }
}