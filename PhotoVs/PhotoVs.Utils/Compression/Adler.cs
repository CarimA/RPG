namespace PhotoVs.Utils.Compression
{
    /// <summary>
    ///     Computes an Adler-32 checksum.
    /// </summary>
    /// <remarks>
    ///     The Adler checksum is similar to a CRC checksum, but faster to compute, though less
    ///     reliable.  It is used in producing RFC1950 compressed streams.  The Adler checksum
    ///     is a required part of the "ZLIB" standard.  Applications will almost never need to
    ///     use this class directly.
    /// </remarks>
    /// <exclude />
    public sealed class Adler
    {
        // largest prime smaller than 65536
        private static readonly uint BASE = 65521;

        // NMAX is the largest n such that 255n(n+1)/2 + (n+1)(BASE-1) <= 2^32-1
        private static readonly int NMAX = 5552;


#pragma warning disable 3001
#pragma warning disable 3002

        /// <summary>
        ///     Calculates the Adler32 checksum.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This is used within ZLIB.  You probably don't need to use this directly.
        ///     </para>
        /// </remarks>
        /// <example>
        ///     To compute an Adler32 checksum on a byte array:
        ///     <code>
        ///    var adler = Adler.Adler32(0, null, 0, 0);
        ///    adler = Adler.Adler32(adler, buffer, index, length);
        ///  </code>
        /// </example>
        internal static uint Adler32(uint adler, byte[] buf, int index, int len)
        {
            if (buf == null)
                return 1;

            var s1 = adler & 0xffff;
            var s2 = adler >> 16 & 0xffff;

            while (len > 0)
            {
                var k = len < NMAX ? len : NMAX;
                len -= k;
                while (k >= 16)
                {
                    //s1 += (buf[index++] & 0xff); s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    s1 += buf[index++];
                    s2 += s1;
                    k -= 16;
                }

                if (k != 0)
                    do
                    {
                        s1 += buf[index++];
                        s2 += s1;
                    } while (--k != 0);

                s1 %= BASE;
                s2 %= BASE;
            }

            return s2 << 16 | s1;
        }
#pragma warning restore 3001
#pragma warning restore 3002
    }
}