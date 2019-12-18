namespace PhotoVs.Utils.Compression
{
    public static class InternalConstants
    {
        internal const int MAX_BITS = 15;
        internal const int BL_CODES = 19;
        internal const int D_CODES = 30;
        internal const int LITERALS = 256;
        internal const int LENGTH_CODES = 29;
        internal const int L_CODES = LITERALS + 1 + LENGTH_CODES;

        // Bit length codes must not exceed MAX_BL_BITS bits
        internal const int MAX_BL_BITS = 7;

        // repeat previous bit length 3-6 times (2 bits of repeat count)
        internal const int REP_3_6 = 16;

        // repeat a zero length 3-10 times  (3 bits of repeat count)
        internal const int REPZ_3_10 = 17;

        // repeat a zero length 11-138 times  (7 bits of repeat count)
        internal const int REPZ_11_138 = 18;
    }
}