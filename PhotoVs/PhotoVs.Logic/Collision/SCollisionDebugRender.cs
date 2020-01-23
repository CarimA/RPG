using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Logic.Camera;
using PhotoVs.Logic.Transforms;
using PhotoVs.Logic.WorldZoning;
using PhotoVs.Models.Assets;
using PhotoVs.Models.ECS;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Logic.Collision
{
    public class SCollisionDebugRender : IDrawableSystem
    {
        private readonly IAssetLoader _assetLoader;
        private readonly SCamera _camera;
        private readonly SMapBoundaryGeneration _mapBoundary;

        private readonly SpriteBatch _spriteBatch;

        public SCollisionDebugRender(SpriteBatch spriteBatch, IAssetLoader assetLoader,
            SMapBoundaryGeneration mapBoundary, SCamera camera)
        {
            _spriteBatch = spriteBatch;
            _assetLoader = assetLoader;
            _mapBoundary = mapBoundary;
            _camera = camera;
        }

        public int Priority { get; set; } = 99;
        public bool Active { get; set; } = true;
        public Type[] Requires { get; } = {typeof(CCollisionBound), typeof(CPosition)};

        public void BeforeDraw(GameTime gameTime)
        {
        }

        public void Draw(GameTime gameTime, IGameObjectCollection entities)
        {
            _camera.Attach(_spriteBatch);

            entities.ForEach(Draw);
            _mapBoundary.GetCollisions()?.ForEach(Draw);
            _mapBoundary.GetScripts()?.ForEach(Draw);
            _mapBoundary.GetZones()?.ForEach(Draw);

            _camera.Detach(_spriteBatch);
        }

        public void AfterDraw(GameTime gameTime)
        {
        }

        public void DrawUI(GameTime gameTime, IGameObjectCollection gameObjectCollection)
        {
        }

        private void Draw(IGameObject entity)
        {
            var position = entity.Components.Get<CPosition>();
            var bounds = entity.Components.Get<CCollisionBound>();
            var boxColor = entity.Components.Has<CSolid>() ? Color.Green : Color.White;


            // draw a box around the inflated boundaries
            DrawBox(position.Position + bounds.InflatedBounds.TopLeft,
                position.Position + bounds.InflatedBounds.TopRight,
                position.Position + bounds.InflatedBounds.BottomLeft,
                position.Position + bounds.InflatedBounds.BottomRight, Color.Red);

            // draw a box around the actual boundaries
            DrawBox(position.Position + bounds.Bounds.TopLeft,
                position.Position + bounds.Bounds.TopRight,
                position.Position + bounds.Bounds.BottomLeft,
                position.Position + bounds.Bounds.BottomRight, Color.Yellow);

            // draw the points
            DrawPolygon(position.Position, bounds.Points, boxColor);

            // draw the center
            DrawBox(position.Position + bounds.Center + new Vector2(-1, -1),
                position.Position + bounds.Center + new Vector2(1, -1),
                position.Position + bounds.Center + new Vector2(-1, 1),
                position.Position + bounds.Center + new Vector2(1, 1), Color.White);
        }

        // todo: move to/create a primitives class
        private void DrawBox(Vector2 topLeft, Vector2 topRight, Vector2 bottomLeft, Vector2 bottomRight, Color color)
        {
            DrawPolygon(Vector2.Zero, new List<Vector2> {topLeft, topRight, bottomRight, bottomLeft}, color);
        }

        private void DrawPolygon(Vector2 origin, List<Vector2> points, Color color)
        {
            Vector2 p1, p2;

            for (var i = 0; i < points.Count; i++)
            {
                p1 = points[i];
                p2 = i + 1 >= points.Count ? points[0] : points[i + 1];
                DrawLine(origin + p1, origin + p2, color);
            }
        }

        // todo: clean
        private void DrawLine(Vector2 start, Vector2 end, Color color, int thickness = 1)
        {
            var edge = end - start;
            // calculate angle to rotate line
            var angle =
                (float) Math.Atan2(edge.Y, edge.X);

            _spriteBatch.Draw(_assetLoader.GetAsset<Texture2D>("Interfaces\\black.png"),
                new Rectangle( // rectangle defines shape of line and position of start of line
                    (int) start.X,
                    (int) start.Y,
                    (int) edge.Length(), //sb will strech the texture to fill this rectangle
                    thickness), //width of line, change this to make thicker line
                null,
                color, //colour of line
                angle, //angle of line (calulated above)
                new Vector2(0, 0), // point in line about which to rotate
                SpriteEffects.None,
                0);
        }
    }
}