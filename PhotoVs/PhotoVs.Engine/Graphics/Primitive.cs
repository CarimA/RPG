
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Utils;

namespace PhotoVs.Engine.Graphics
{
    public class Primitive
    {
        private readonly SpriteBatch _spriteBatch;
        private readonly Texture2D _pixel;

        public Primitive(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;

            _pixel = new Texture2D(graphicsDevice, 1, 1);
            _pixel.SetData(new Color[] {Color.White});
        }

        public void DrawLine(Vector2 p1, Vector2 p2, Color color, int thickness = 1)
        {
            var edge = p2 - p1;
            var angle = (float) Math.Atan2(edge.Y, edge.X);
            _spriteBatch.Draw(_pixel, new Rectangle((int) p1.X, (int) p1.Y, (int) edge.Length(), thickness), null,
                color, angle, Vector2.Zero, SpriteEffects.None, 0);
        }

        public void DrawPolygon(List<Vector2> points, Color color, int thickness = 1)
        {
            Vector2 p1, p2;
            for (var i = 0; i < points.Count; i++)
            {
                p1 = points[i];
                p2 = i + 1 >= points.Count
                    ? points[0]
                    : points[i + 1];
                DrawLine(p1, p2, color, thickness);
            }
        }

        public void DrawPolygon(Vector2 origin, List<Vector2> points, Color color, int thickness = 1)
        {
            Vector2 p1, p2;
            for (var i = 0; i < points.Count; i++)
            {
                p1 = points[i];
                p2 = i + 1 >= points.Count
                    ? points[0]
                    : points[i + 1];
                DrawLine(origin + p1, origin + p2, color, thickness);
            }
        }

        public void DrawBox(float x, float y, float width, float height, Color color, int thickness = 1)
        {
            DrawBox(new RectangleF(x, y, width, height), color, thickness);
        }

        public void DrawBox(int x, int y, int width, int height, Color color, int thickness = 1)
        {
            DrawBox(new Rectangle(x, y, width, height), color, thickness);
        }

        public void DrawBox(RectangleF rect, Color color, int thickness = 1)
        {
            var topLeft = new Vector2(rect.Left, rect.Top);
            var topRight = new Vector2(rect.Right, rect.Top);
            var bottomLeft = new Vector2(rect.Left, rect.Bottom);
            var bottomRight = new Vector2(rect.Right, rect.Bottom);

            DrawLine(topLeft, topRight, color, thickness);
            DrawLine(topRight, bottomRight, color, thickness);
            DrawLine(bottomRight, bottomLeft, color, thickness);
            DrawLine(bottomLeft, topLeft, color, thickness);
        }

        public void DrawBox(Rectangle rect, Color color, int thickness = 1)
        {
            var topLeft = new Vector2(rect.Left, rect.Top);
            var topRight = new Vector2(rect.Right, rect.Top);
            var bottomLeft = new Vector2(rect.Left, rect.Bottom);
            var bottomRight = new Vector2(rect.Right, rect.Bottom);

            DrawLine(topLeft, topRight, color, thickness);
            DrawLine(topRight, bottomRight, color, thickness);
            DrawLine(bottomRight, bottomLeft, color, thickness);
            DrawLine(bottomLeft, topLeft, color, thickness);
        }
    }
}
