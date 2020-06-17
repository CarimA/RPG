using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.ECS.GameObjects;
using PhotoVs.Engine.ECS.Systems;
using PhotoVs.Logic.Mechanics.Camera.Systems;
using System;

namespace PhotoVs.Logic.Mechanics.World.Systems
{
    public class SRenderOverworld : IDrawableSystem
    {
        private readonly Overworld _overworld;
        private readonly SpriteBatch _spriteBatch;
        private readonly SCamera _camera;

        private IGameObjectCollection _gameObjects;

        public int Priority { get; set; } = 0;
        public bool Active { get; set; } = true;

        public Type[] Requires { get; } =
        {
        };

        public SRenderOverworld(Overworld overworld, SpriteBatch spriteBatch, SCamera camera)
        {
            _overworld = overworld;
            _spriteBatch = spriteBatch;
            _camera = camera;
        }

        public void BeforeDraw(GameTime gameTime)
        {

        }

        public void Draw(GameTime gameTime, IGameObjectCollection gameObjects)
        {
            _gameObjects = gameObjects;
            _overworld.GetMap()
                .Draw(_spriteBatch,
                    gameTime,
                    _camera,
                    EntityDraw);
        }

        public void EntityDraw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var gameObject in _gameObjects)
            {

            }
        }

        public void AfterDraw(GameTime gameTime)
        {

        }

        public void DrawUI(GameTime gameTime, IGameObjectCollection gameObjectCollection, Matrix uiOrigin)
        {


        }
    }
}
