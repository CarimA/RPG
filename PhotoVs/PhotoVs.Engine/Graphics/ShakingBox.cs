using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Utils;

namespace PhotoVs.Engine.Graphics
{
    public class ShakingBox
    {
        private readonly List<PolygonPrimitive> _blackPrimitives;
        private readonly List<RectangleF> _boxes;
        private readonly List<PolygonPrimitive> _whitePrimitives;

        public ShakingBox(SpriteBatch spriteBatch, List<RectangleF> boxes)
        {
            _boxes = boxes;
            _whitePrimitives = new List<PolygonPrimitive>();
            _blackPrimitives = new List<PolygonPrimitive>();

            for (var i = 0; i < boxes.Count; i++)
            {
                _whitePrimitives.Add(new PolygonPrimitive(spriteBatch.GraphicsDevice, Color.White));
                _blackPrimitives.Add(new PolygonPrimitive(spriteBatch.GraphicsDevice, Color.Black));
            }

            Update(new GameTime(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(0)));
            ;
        }

        public void Update(GameTime gameTime)
        {
            var time = (float)gameTime.TotalGameTime.TotalMilliseconds;
            var scaleA = time % 360f / 360f;
            scaleA = scaleA >= 0.5f ? 1f - scaleA : scaleA;

            var scaleB = (time + 230f) % 360f / 360f;
            scaleB = scaleB >= 0.5f ? 1f - scaleB : scaleB;

            var scaleC = (time + 130f) % 360f / 360f;
            scaleC = scaleC >= 0.5f ? 1f - scaleC : scaleC;

            var diffA = scaleA * 7;
            var diffB = scaleB * 5;
            var diffC = scaleC * 9;
            var diffSmallA = scaleA * 11;
            var diffSmallB = scaleB * 10;
            var diffSmallC = scaleC * 12;

            for (var i = 0; i < _boxes.Count; i++)
            {
                var box = _boxes[i];
                var white = _whitePrimitives[i];
                var black = _blackPrimitives[i];

                var rect = box;

                if (i % 2 == 0)
                {
                    var topLeft = rect.TopLeft + new Vector2(-diffA, -diffC);
                    var topRight = rect.TopRight + new Vector2(diffB, -diffA);
                    var bottomRight = rect.BottomRight + new Vector2(diffC, -diffC);
                    var bottomLeft = rect.BottomLeft + new Vector2(-diffB, -diffB);

                    var topLeftS = topLeft + new Vector2(-diffSmallA, -diffSmallC);
                    var topRightS = topRight + new Vector2(diffSmallB, -diffSmallA);
                    var bottomRightS = bottomRight + new Vector2(diffSmallC, diffSmallC);
                    var bottomLeftS = bottomLeft + new Vector2(-diffSmallB, diffSmallB);

                    black.SetPoints(new List<Vector2> { topLeft, topRight, bottomRight, bottomLeft });
                    white.SetPoints(new List<Vector2> { topLeftS, topRightS, bottomRightS, bottomLeftS });
                }
                else
                {
                    var topLeft = rect.TopLeft + new Vector2(-diffC, -diffB);
                    var topRight = rect.TopRight + new Vector2(diffA, -diffC);
                    var bottomRight = rect.BottomRight + new Vector2(diffA, -diffC);
                    var bottomLeft = rect.BottomLeft + new Vector2(-diffB, -diffA);

                    var topLeftS = topLeft + new Vector2(-diffSmallC, -diffSmallB);
                    var topRightS = topRight + new Vector2(diffSmallA, -diffSmallC);
                    var bottomRightS = bottomRight + new Vector2(diffSmallA, diffSmallA);
                    var bottomLeftS = bottomLeft + new Vector2(-diffSmallB, diffSmallA);

                    black.SetPoints(new List<Vector2> { topLeft, topRight, bottomRight, bottomLeft });
                    white.SetPoints(new List<Vector2> { topLeftS, topRightS, bottomRightS, bottomLeftS });
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            foreach (var white in _whitePrimitives)
                white.Draw();

            foreach (var black in _blackPrimitives)
                black.Draw();
        }
    }
}