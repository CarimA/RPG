﻿using System;

namespace PhotoVs.Utils.Compression
{
    public sealed class InflateBlocks
    {
        private const int MANY = 1440;

        // Table for deflate from PKZIP's appnote.txt.
        internal static readonly int[] border = {16, 17, 18, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 14, 1, 15};

        internal ZlibCodec _codec; // pointer back to this zlib stream
        internal int[] bb = new int[1]; // bit length tree depth
        internal int bitb; // bit buffer

        // mode independent information
        internal int bitk; // bits in bit buffer
        internal int[] blens; // bit lengths of codes
        internal uint check; // check on output
        internal object checkfn; // check function

        internal InflateCodes codes = new InflateCodes(); // if CODES, current state
        internal int end; // one byte after sliding window
        internal int[] hufts; // single malloc for tree space
        internal int index; // index into blens (or border)

        internal InfTree inftree = new InfTree();

        internal int last; // true if this block is the last block

        internal int left; // if STORED, bytes left to copy

        private InflateBlockMode mode; // current inflate_block mode
        internal int readAt; // window read pointer

        internal int table; // table lengths (14 bits)
        internal int[] tb = new int[1]; // bit length decoding tree
        internal byte[] window; // sliding window
        internal int writeAt; // window write pointer

        internal InflateBlocks(ZlibCodec codec, object checkfn, int w)
        {
            _codec = codec;
            hufts = new int[MANY * 3];
            window = new byte[w];
            end = w;
            this.checkfn = checkfn;
            mode = InflateBlockMode.TYPE;
            Reset();
        }

        internal uint Reset()
        {
            var oldCheck = check;
            mode = InflateBlockMode.TYPE;
            bitk = 0;
            bitb = 0;
            readAt = writeAt = 0;

            if (checkfn != null)
                _codec._Adler32 = check = Adler.Adler32(0, null, 0, 0);
            return oldCheck;
        }


        internal int Process(int r)
        {
            int t; // temporary storage
            int b; // bit buffer
            int k; // bits in bit buffer
            int p; // input data pointer
            int n; // bytes available there
            int q; // output window write pointer
            int m; // bytes to end of window or read pointer

            // copy input/output information to locals (UPDATE macro restores)

            p = _codec.NextIn;
            n = _codec.AvailableBytesIn;
            b = bitb;
            k = bitk;

            q = writeAt;
            m = q < readAt ? readAt - q - 1 : end - q;


            // process input based on current state
            while (true)
                switch (mode)
                {
                    case InflateBlockMode.TYPE:

                        while (k < 3)
                        {
                            if (n != 0)
                            {
                                r = ZlibConstants.Z_OK;
                            }
                            else
                            {
                                bitb = b;
                                bitk = k;
                                _codec.AvailableBytesIn = n;
                                _codec.TotalBytesIn += p - _codec.NextIn;
                                _codec.NextIn = p;
                                writeAt = q;
                                return Flush(r);
                            }

                            n--;
                            b |= (_codec.InputBuffer[p++] & 0xff) << k;
                            k += 8;
                        }

                        t = b & 7;
                        last = t & 1;

                        switch ((uint) t >> 1)
                        {
                            case 0: // stored
                                b >>= 3;
                                k -= 3;
                                t = k & 7; // go to byte boundary
                                b >>= t;
                                k -= t;
                                mode = InflateBlockMode.LENS; // get length of stored block
                                break;

                            case 1: // fixed
                                var bl = new int[1];
                                var bd = new int[1];
                                var tl = new int[1][];
                                var td = new int[1][];
                                InfTree.InflateTreesFixed(bl, bd, tl, td, _codec);
                                codes.Init(bl[0], bd[0], tl[0], 0, td[0], 0);
                                b >>= 3;
                                k -= 3;
                                mode = InflateBlockMode.CODES;
                                break;

                            case 2: // dynamic
                                b >>= 3;
                                k -= 3;
                                mode = InflateBlockMode.TABLE;
                                break;

                            case 3: // illegal
                                b >>= 3;
                                k -= 3;
                                mode = InflateBlockMode.BAD;
                                _codec.Message = "invalid block type";
                                r = ZlibConstants.Z_DATA_ERROR;
                                bitb = b;
                                bitk = k;
                                _codec.AvailableBytesIn = n;
                                _codec.TotalBytesIn += p - _codec.NextIn;
                                _codec.NextIn = p;
                                writeAt = q;
                                return Flush(r);
                        }

                        break;

                    case InflateBlockMode.LENS:

                        while (k < 32)
                        {
                            if (n != 0)
                            {
                                r = ZlibConstants.Z_OK;
                            }
                            else
                            {
                                bitb = b;
                                bitk = k;
                                _codec.AvailableBytesIn = n;
                                _codec.TotalBytesIn += p - _codec.NextIn;
                                _codec.NextIn = p;
                                writeAt = q;
                                return Flush(r);
                            }

                            ;
                            n--;
                            b |= (_codec.InputBuffer[p++] & 0xff) << k;
                            k += 8;
                        }

                        if (((~b >> 16) & 0xffff) != (b & 0xffff))
                        {
                            mode = InflateBlockMode.BAD;
                            _codec.Message = "invalid stored block lengths";
                            r = ZlibConstants.Z_DATA_ERROR;

                            bitb = b;
                            bitk = k;
                            _codec.AvailableBytesIn = n;
                            _codec.TotalBytesIn += p - _codec.NextIn;
                            _codec.NextIn = p;
                            writeAt = q;
                            return Flush(r);
                        }

                        left = b & 0xffff;
                        b = k = 0; // dump bits
                        mode = left != 0 ? InflateBlockMode.STORED :
                            last != 0 ? InflateBlockMode.DRY : InflateBlockMode.TYPE;
                        break;

                    case InflateBlockMode.STORED:
                        if (n == 0)
                        {
                            bitb = b;
                            bitk = k;
                            _codec.AvailableBytesIn = n;
                            _codec.TotalBytesIn += p - _codec.NextIn;
                            _codec.NextIn = p;
                            writeAt = q;
                            return Flush(r);
                        }

                        if (m == 0)
                        {
                            if (q == end && readAt != 0)
                            {
                                q = 0;
                                m = q < readAt ? readAt - q - 1 : end - q;
                            }

                            if (m == 0)
                            {
                                writeAt = q;
                                r = Flush(r);
                                q = writeAt;
                                m = q < readAt ? readAt - q - 1 : end - q;
                                if (q == end && readAt != 0)
                                {
                                    q = 0;
                                    m = q < readAt ? readAt - q - 1 : end - q;
                                }

                                if (m == 0)
                                {
                                    bitb = b;
                                    bitk = k;
                                    _codec.AvailableBytesIn = n;
                                    _codec.TotalBytesIn += p - _codec.NextIn;
                                    _codec.NextIn = p;
                                    writeAt = q;
                                    return Flush(r);
                                }
                            }
                        }

                        r = ZlibConstants.Z_OK;

                        t = left;
                        if (t > n)
                            t = n;
                        if (t > m)
                            t = m;
                        Array.Copy(_codec.InputBuffer, p, window, q, t);
                        p += t;
                        n -= t;
                        q += t;
                        m -= t;
                        if ((left -= t) != 0)
                            break;
                        mode = last != 0 ? InflateBlockMode.DRY : InflateBlockMode.TYPE;
                        break;

                    case InflateBlockMode.TABLE:

                        while (k < 14)
                        {
                            if (n != 0)
                            {
                                r = ZlibConstants.Z_OK;
                            }
                            else
                            {
                                bitb = b;
                                bitk = k;
                                _codec.AvailableBytesIn = n;
                                _codec.TotalBytesIn += p - _codec.NextIn;
                                _codec.NextIn = p;
                                writeAt = q;
                                return Flush(r);
                            }

                            n--;
                            b |= (_codec.InputBuffer[p++] & 0xff) << k;
                            k += 8;
                        }

                        table = t = b & 0x3fff;
                        if ((t & 0x1f) > 29 || ((t >> 5) & 0x1f) > 29)
                        {
                            mode = InflateBlockMode.BAD;
                            _codec.Message = "too many length or distance symbols";
                            r = ZlibConstants.Z_DATA_ERROR;

                            bitb = b;
                            bitk = k;
                            _codec.AvailableBytesIn = n;
                            _codec.TotalBytesIn += p - _codec.NextIn;
                            _codec.NextIn = p;
                            writeAt = q;
                            return Flush(r);
                        }

                        t = 258 + (t & 0x1f) + ((t >> 5) & 0x1f);
                        if (blens == null || blens.Length < t)
                            blens = new int[t];
                        else
                            Array.Clear(blens, 0, t);
                        // for (int i = 0; i < t; i++)
                        // {
                        //     blens[i] = 0;
                        // }

                        b >>= 14;
                        k -= 14;


                        index = 0;
                        mode = InflateBlockMode.BTREE;
                        goto case InflateBlockMode.BTREE;

                    case InflateBlockMode.BTREE:
                        while (index < 4 + (table >> 10))
                        {
                            while (k < 3)
                            {
                                if (n != 0)
                                {
                                    r = ZlibConstants.Z_OK;
                                }
                                else
                                {
                                    bitb = b;
                                    bitk = k;
                                    _codec.AvailableBytesIn = n;
                                    _codec.TotalBytesIn += p - _codec.NextIn;
                                    _codec.NextIn = p;
                                    writeAt = q;
                                    return Flush(r);
                                }

                                n--;
                                b |= (_codec.InputBuffer[p++] & 0xff) << k;
                                k += 8;
                            }

                            blens[border[index++]] = b & 7;

                            b >>= 3;
                            k -= 3;
                        }

                        while (index < 19)
                            blens[border[index++]] = 0;

                        bb[0] = 7;
                        t = inftree.InflateTreesBits(blens, bb, tb, hufts, _codec);
                        if (t != ZlibConstants.Z_OK)
                        {
                            r = t;
                            if (r == ZlibConstants.Z_DATA_ERROR)
                            {
                                blens = null;
                                mode = InflateBlockMode.BAD;
                            }

                            bitb = b;
                            bitk = k;
                            _codec.AvailableBytesIn = n;
                            _codec.TotalBytesIn += p - _codec.NextIn;
                            _codec.NextIn = p;
                            writeAt = q;
                            return Flush(r);
                        }

                        index = 0;
                        mode = InflateBlockMode.DTREE;
                        goto case InflateBlockMode.DTREE;

                    case InflateBlockMode.DTREE:
                        while (true)
                        {
                            t = table;
                            if (!(index < 258 + (t & 0x1f) + ((t >> 5) & 0x1f)))
                                break;

                            int i, j, c;

                            t = bb[0];

                            while (k < t)
                            {
                                if (n != 0)
                                {
                                    r = ZlibConstants.Z_OK;
                                }
                                else
                                {
                                    bitb = b;
                                    bitk = k;
                                    _codec.AvailableBytesIn = n;
                                    _codec.TotalBytesIn += p - _codec.NextIn;
                                    _codec.NextIn = p;
                                    writeAt = q;
                                    return Flush(r);
                                }

                                n--;
                                b |= (_codec.InputBuffer[p++] & 0xff) << k;
                                k += 8;
                            }

                            t = hufts[(tb[0] + (b & InternalInflateConstants.InflateMask[t])) * 3 + 1];
                            c = hufts[(tb[0] + (b & InternalInflateConstants.InflateMask[t])) * 3 + 2];

                            if (c < 16)
                            {
                                b >>= t;
                                k -= t;
                                blens[index++] = c;
                            }
                            else
                            {
                                // c == 16..18
                                i = c == 18 ? 7 : c - 14;
                                j = c == 18 ? 11 : 3;

                                while (k < t + i)
                                {
                                    if (n != 0)
                                    {
                                        r = ZlibConstants.Z_OK;
                                    }
                                    else
                                    {
                                        bitb = b;
                                        bitk = k;
                                        _codec.AvailableBytesIn = n;
                                        _codec.TotalBytesIn += p - _codec.NextIn;
                                        _codec.NextIn = p;
                                        writeAt = q;
                                        return Flush(r);
                                    }

                                    n--;
                                    b |= (_codec.InputBuffer[p++] & 0xff) << k;
                                    k += 8;
                                }

                                b >>= t;
                                k -= t;

                                j += b & InternalInflateConstants.InflateMask[i];

                                b >>= i;
                                k -= i;

                                i = index;
                                t = table;
                                if (i + j > 258 + (t & 0x1f) + ((t >> 5) & 0x1f) || c == 16 && i < 1)
                                {
                                    blens = null;
                                    mode = InflateBlockMode.BAD;
                                    _codec.Message = "invalid bit length repeat";
                                    r = ZlibConstants.Z_DATA_ERROR;

                                    bitb = b;
                                    bitk = k;
                                    _codec.AvailableBytesIn = n;
                                    _codec.TotalBytesIn += p - _codec.NextIn;
                                    _codec.NextIn = p;
                                    writeAt = q;
                                    return Flush(r);
                                }

                                c = c == 16 ? blens[i - 1] : 0;
                                do
                                {
                                    blens[i++] = c;
                                } while (--j != 0);

                                index = i;
                            }
                        }

                        tb[0] = -1;
                    {
                        int[] bl = {9}; // must be <= 9 for lookahead assumptions
                        int[] bd = {6}; // must be <= 9 for lookahead assumptions
                        var tl = new int[1];
                        var td = new int[1];

                        t = table;
                        t = inftree.InflateTreesDynamic(257 + (t & 0x1f), 1 + ((t >> 5) & 0x1f), blens, bl, bd, tl, td,
                            hufts, _codec);

                        if (t != ZlibConstants.Z_OK)
                        {
                            if (t == ZlibConstants.Z_DATA_ERROR)
                            {
                                blens = null;
                                mode = InflateBlockMode.BAD;
                            }

                            r = t;

                            bitb = b;
                            bitk = k;
                            _codec.AvailableBytesIn = n;
                            _codec.TotalBytesIn += p - _codec.NextIn;
                            _codec.NextIn = p;
                            writeAt = q;
                            return Flush(r);
                        }

                        codes.Init(bl[0], bd[0], hufts, tl[0], hufts, td[0]);
                    }
                        mode = InflateBlockMode.CODES;
                        goto case InflateBlockMode.CODES;

                    case InflateBlockMode.CODES:
                        bitb = b;
                        bitk = k;
                        _codec.AvailableBytesIn = n;
                        _codec.TotalBytesIn += p - _codec.NextIn;
                        _codec.NextIn = p;
                        writeAt = q;

                        r = codes.Process(this, r);
                        if (r != ZlibConstants.Z_STREAM_END)
                            return Flush(r);

                        r = ZlibConstants.Z_OK;
                        p = _codec.NextIn;
                        n = _codec.AvailableBytesIn;
                        b = bitb;
                        k = bitk;
                        q = writeAt;
                        m = q < readAt ? readAt - q - 1 : end - q;

                        if (last == 0)
                        {
                            mode = InflateBlockMode.TYPE;
                            break;
                        }

                        mode = InflateBlockMode.DRY;
                        goto case InflateBlockMode.DRY;

                    case InflateBlockMode.DRY:
                        writeAt = q;
                        r = Flush(r);
                        q = writeAt;
                        if (readAt != writeAt)
                        {
                            bitb = b;
                            bitk = k;
                            _codec.AvailableBytesIn = n;
                            _codec.TotalBytesIn += p - _codec.NextIn;
                            _codec.NextIn = p;
                            writeAt = q;
                            return Flush(r);
                        }

                        mode = InflateBlockMode.DONE;
                        goto case InflateBlockMode.DONE;

                    case InflateBlockMode.DONE:
                        r = ZlibConstants.Z_STREAM_END;
                        bitb = b;
                        bitk = k;
                        _codec.AvailableBytesIn = n;
                        _codec.TotalBytesIn += p - _codec.NextIn;
                        _codec.NextIn = p;
                        writeAt = q;
                        return Flush(r);

                    case InflateBlockMode.BAD:
                        r = ZlibConstants.Z_DATA_ERROR;

                        bitb = b;
                        bitk = k;
                        _codec.AvailableBytesIn = n;
                        _codec.TotalBytesIn += p - _codec.NextIn;
                        _codec.NextIn = p;
                        writeAt = q;
                        return Flush(r);


                    default:
                        r = ZlibConstants.Z_STREAM_ERROR;

                        bitb = b;
                        bitk = k;
                        _codec.AvailableBytesIn = n;
                        _codec.TotalBytesIn += p - _codec.NextIn;
                        _codec.NextIn = p;
                        writeAt = q;
                        return Flush(r);
                }
        }


        internal void Free()
        {
            Reset();
            window = null;
            hufts = null;
        }

        internal void SetDictionary(byte[] d, int start, int n)
        {
            Array.Copy(d, start, window, 0, n);
            readAt = writeAt = n;
        }

        // Returns true if inflate is currently at the end of a block generated
        // by Z_SYNC_FLUSH or Z_FULL_FLUSH.
        internal int SyncPoint()
        {
            return mode == InflateBlockMode.LENS ? 1 : 0;
        }

        // copy as much as possible from the sliding window to the output area
        internal int Flush(int r)
        {
            int nBytes;

            for (var pass = 0; pass < 2; pass++)
            {
                if (pass == 0)
                    // compute number of bytes to copy as far as end of window
                    nBytes = (readAt <= writeAt ? writeAt : end) - readAt;
                else
                    // compute bytes to copy
                    nBytes = writeAt - readAt;

                // workitem 8870
                if (nBytes == 0)
                {
                    if (r == ZlibConstants.Z_BUF_ERROR)
                        r = ZlibConstants.Z_OK;
                    return r;
                }

                if (nBytes > _codec.AvailableBytesOut)
                    nBytes = _codec.AvailableBytesOut;

                if (nBytes != 0 && r == ZlibConstants.Z_BUF_ERROR)
                    r = ZlibConstants.Z_OK;

                // update counters
                _codec.AvailableBytesOut -= nBytes;
                _codec.TotalBytesOut += nBytes;

                // update check information
                if (checkfn != null)
                    _codec._Adler32 = check = Adler.Adler32(check, window, readAt, nBytes);

                // copy as far as end of window
                Array.Copy(window, readAt, _codec.OutputBuffer, _codec.NextOut, nBytes);
                _codec.NextOut += nBytes;
                readAt += nBytes;

                // see if more to copy at beginning of window
                if (readAt == end && pass == 0)
                {
                    // wrap pointers
                    readAt = 0;
                    if (writeAt == end)
                        writeAt = 0;
                }
                else
                {
                    pass++;
                }
            }

            // done
            return r;
        }

        private enum InflateBlockMode
        {
            TYPE = 0, // get type bits (3, including end bit)
            LENS = 1, // get lengths for stored
            STORED = 2, // processing stored block
            TABLE = 3, // get table lengths
            BTREE = 4, // get bit lengths tree for a dynamic block
            DTREE = 5, // get length, distance trees for a dynamic block
            CODES = 6, // processing fixed or dynamic block
            DRY = 7, // output remaining window bytes
            DONE = 8, // finished last block, done
            BAD = 9 // ot a data error--stuck here
        }
    }
}