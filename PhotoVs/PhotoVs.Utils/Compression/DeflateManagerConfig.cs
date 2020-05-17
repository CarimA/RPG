namespace PhotoVs.Utils.Compression
{
    public class DeflateManagerConfig
    {
        private static readonly DeflateManagerConfig[] Table;

        internal DeflateFlavor Flavor;

        // Use a faster search when the previous match is longer than this
        internal int GoodLength; // reduce lazy search above this match length

        // To speed up deflation, hash chains are never searched beyond this
        // length.  A higher limit improves compression ratio but degrades the speed.

        internal int MaxChainLength;

        // Attempt to find a better match only when the current match is
        // strictly smaller than this value. This mechanism is used only for
        // compression levels >= 4.  For levels 1,2,3: MaxLazy is actually
        // MaxInsertLength. (See DeflateFast)

        internal int MaxLazy; // do not perform lazy search above this match length

        internal int NiceLength; // quit search above this match length


        static DeflateManagerConfig()
        {
            Table = new[]
            {
                new DeflateManagerConfig(0, 0, 0, 0, DeflateFlavor.Store),
                new DeflateManagerConfig(4, 4, 8, 4, DeflateFlavor.Fast),
                new DeflateManagerConfig(4, 5, 16, 8, DeflateFlavor.Fast),
                new DeflateManagerConfig(4, 6, 32, 32, DeflateFlavor.Fast),

                new DeflateManagerConfig(4, 4, 16, 16, DeflateFlavor.Slow),
                new DeflateManagerConfig(8, 16, 32, 32, DeflateFlavor.Slow),
                new DeflateManagerConfig(8, 16, 128, 128, DeflateFlavor.Slow),
                new DeflateManagerConfig(8, 32, 128, 256, DeflateFlavor.Slow),
                new DeflateManagerConfig(32, 128, 258, 1024, DeflateFlavor.Slow),
                new DeflateManagerConfig(32, 258, 258, 4096, DeflateFlavor.Slow)
            };
        }

        private DeflateManagerConfig(int goodLength, int maxLazy, int niceLength, int maxChainLength,
            DeflateFlavor flavor)
        {
            GoodLength = goodLength;
            MaxLazy = maxLazy;
            NiceLength = niceLength;
            MaxChainLength = maxChainLength;
            Flavor = flavor;
        }

        internal static DeflateManagerConfig Lookup(CompressionLevel level)
        {
            return Table[(int)level];
        }
    }
}