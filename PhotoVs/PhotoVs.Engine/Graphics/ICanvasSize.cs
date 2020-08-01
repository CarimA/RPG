using System;
using Microsoft.Xna.Framework;

namespace PhotoVs.Engine.Graphics
{
    public interface ICanvasSize
    {
        int Width { get; }
        int Height { get; }
        int MaxWidth { get; }
        int MaxHeight { get; }
        Rectangle DisplayRectangle { get; }
        int DisplayWidth { get; }
        int DisplayHeight { get; }
        Action OnResize { get; set; }
    }
}