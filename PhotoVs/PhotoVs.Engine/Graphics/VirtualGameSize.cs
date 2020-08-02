namespace PhotoVs.Engine.Graphics
{
    public class VirtualGameSize
    {
        public int Width { get; }
        public int Height { get; }
        public int BackbufferHeight { get; }

        public VirtualGameSize(int width, int height, int backbufferHeight)
        {
            Width = width;
            Height = height;
            BackbufferHeight = backbufferHeight;
        }
    }
}