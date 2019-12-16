using Microsoft.Xna.Framework;
using PhotoVs.Math;

namespace PhotoVs.Graphics
{
    public class CanvasSize
    {
        private readonly int _height;
        private readonly int _width;

        public CanvasSize(int width, int height)
        {
            _width = width;
            _height = height;
        }

        public int GetWidth()
        {
            return _width;
        }

        public int GetHeight()
        {
            return _height;
        }

        public Size2 GetSize2()
        {
            return new Size2(GetWidth(), GetHeight());
        }

        public Vector2 GetVector2()
        {
            return new Vector2(GetWidth(), GetHeight());
        }
    }
}