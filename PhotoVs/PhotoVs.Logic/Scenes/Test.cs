﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.ECS;
using PhotoVs.Engine.Graphics;
using PhotoVs.Logic.Mechanics;
using PhotoVs.Logic.Mechanics.World;

namespace PhotoVs.Logic.Scenes
{
    public class Test : Scene
    {
        private readonly Camera _camera;
        private readonly SpriteBatch _spriteBatch;
        private WebView test;

        public Test(DrawMap drawMap, Movement movement, Camera camera, IRenderer renderer, IAssetLoader assetLoader, SpriteBatch spriteBatch, CanvasSize canvasSize, GameDate gameDate)
        {
            _camera = camera;
            _spriteBatch = spriteBatch;
            RegisterSystem(drawMap.Draw);

            RegisterSystem(drawMap.DebugDrawCollidableEntities, 999);

            RegisterSystem(camera.DebugDraw, 999);
            //RegisterSystem(DebugDrawHtml, 1000);

            RegisterSystem(movement.ProcessMovement, 0);

            test = new WebView(renderer, assetLoader, spriteBatch, 
                canvasSize.VirtualCurrentWidth, canvasSize.VirtualCurrentHeight, 
                "file://content/ui/html/test.html");
            
            canvasSize.OnResize += () =>
            {
                test.Resize(canvasSize.VirtualCurrentWidth, canvasSize.VirtualCurrentHeight);
            };

            gameDate.DisableTimeFlow();
        }


        [System(RunOn.Draw)]
        public void DebugDrawHtml(GameTime gameTime, GameObjectList gameObjects)
        {
            var tex = test.Texture;
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _camera.UITransform);
            _spriteBatch.Draw(tex, Vector2.Zero, Color.White);
            _spriteBatch.End();
        }
    }
}
