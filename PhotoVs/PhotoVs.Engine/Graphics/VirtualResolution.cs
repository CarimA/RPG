namespace PhotoVs.Engine.Graphics
{
    public class VirtualResolution
    {
        public int MinWidth { get; }
        public int MinHeight { get; }
        public int MaxWidth { get; }
        public int MaxHeight { get; }

        public VirtualResolution(int minWidth, int minHeight, int maxWidth, int maxHeight)
        {
            MinWidth = minWidth;
            MinHeight = minHeight;
            MaxWidth = maxWidth;
            MaxHeight = maxHeight;
        }
    }
}