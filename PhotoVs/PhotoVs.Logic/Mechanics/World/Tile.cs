using Microsoft.Xna.Framework.Graphics;

namespace PhotoVs.Logic.Mechanics.World
{
    public readonly struct Tile
    {
        public readonly int X { get; }
        public readonly int Y { get; }
        public readonly int SourceX { get; }
        public readonly int SourceY { get; }
        public readonly Texture2D Texture { get; }
        public readonly Texture2D Material { get; }

        public Tile(int x, int y, int sourceX, int sourceY, Texture2D texture, Texture2D material)
        {
            X = x;
            Y = y;
            SourceX = sourceX;
            SourceY = sourceY;
            Texture = texture;
            Material = material;
        }
    }
}