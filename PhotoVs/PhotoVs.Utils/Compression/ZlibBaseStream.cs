using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PhotoVs.Utils.Compression
{
    public class ZlibBaseStream : Stream
    {
        internal ZlibCodec _z; // deferred init... new ZlibCodec();

        internal StreamMode _streamMode = StreamMode.Undefined;
        internal FlushType _flushMode;
        internal ZlibStreamFlavor _flavor;
        internal CompressionMode _compressionMode;
        internal CompressionLevel _level;
        internal bool _leaveOpen;
        internal byte[] _workingBuffer;
        internal int _bufferSize = ZlibConstants.WorkingBufferSizeDefault;
        internal byte[] _buf1 = new byte[1];

        internal Stream _stream;
        internal CompressionStrategy Strategy = CompressionStrategy.Default;

        // workitem 7159
        private readonly CRC32 crc;
        internal string _GzipFileName;
        internal string _GzipComment;
        internal DateTime _GzipMtime;
        internal int _gzipHeaderByteCount;

        internal int Crc32
        {
            get
            {
                if (crc == null)
                    return 0;
                return crc.Crc32Result;
            }
        }

        internal ZlibBaseStream(Stream stream,
            CompressionMode compressionMode,
            CompressionLevel level,
            ZlibStreamFlavor flavor,
            bool leaveOpen)
        {
            _flushMode = FlushType.None;
            //this._workingBuffer = new byte[WORKING_BUFFER_SIZE_DEFAULT];
            _stream = stream;
            _leaveOpen = leaveOpen;
            _compressionMode = compressionMode;
            _flavor = flavor;
            _level = level;
            // workitem 7159
            if (flavor == ZlibStreamFlavor.GZIP)
                crc = new CRC32();
        }


        protected internal bool WantCompress => _compressionMode == CompressionMode.Compress;

        private ZlibCodec Z
        {
            get
            {
                if (_z == null)
                {
                    var wantRfc1950Header = _flavor == ZlibStreamFlavor.ZLIB;
                    _z = new ZlibCodec();
                    if (_compressionMode == CompressionMode.Decompress)
                    {
                        _z.InitializeInflate(wantRfc1950Header);
                    }
                    else
                    {
                        _z.Strategy = Strategy;
                        _z.InitializeDeflate(_level, wantRfc1950Header);
                    }
                }

                return _z;
            }
        }


        private byte[] WorkingBuffer
        {
            get
            {
                if (_workingBuffer == null)
                    _workingBuffer = new byte[_bufferSize];
                return _workingBuffer;
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            // workitem 7159
            // calculate the CRC on the unccompressed data  (before writing)
            if (crc != null)
                crc.SlurpBlock(buffer, offset, count);

            if (_streamMode == StreamMode.Undefined)
                _streamMode = StreamMode.Writer;
            else if (_streamMode != StreamMode.Writer)
                throw new ZlibException("Cannot Write after Reading.");

            if (count == 0)
                return;

            // first reference of Z property will initialize the private var _z
            Z.InputBuffer = buffer;
            _z.NextIn = offset;
            _z.AvailableBytesIn = count;
            bool done;
            do
            {
                _z.OutputBuffer = WorkingBuffer;
                _z.NextOut = 0;
                _z.AvailableBytesOut = _workingBuffer.Length;
                var rc = WantCompress
                    ? _z.Deflate(_flushMode)
                    : _z.Inflate(_flushMode);
                if (rc != ZlibConstants.Z_OK && rc != ZlibConstants.Z_STREAM_END)
                    throw new ZlibException((WantCompress ? "de" : "in") + "flating: " + _z.Message);

                //if (_workingBuffer.Length - _z.AvailableBytesOut > 0)
                _stream.Write(_workingBuffer, 0, _workingBuffer.Length - _z.AvailableBytesOut);

                done = _z.AvailableBytesIn == 0 && _z.AvailableBytesOut != 0;

                // If GZIP and de-compress, we're done when 8 bytes remain.
                if (_flavor == ZlibStreamFlavor.GZIP && !WantCompress)
                    done = _z.AvailableBytesIn == 8 && _z.AvailableBytesOut != 0;
            } while (!done);
        }

        public override void Flush()
        {
            _stream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
            //_outStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _stream.SetLength(value);
        }


#if NOT
        internal int Read()
        {
            if (Read(_buf1, 0, 1) == 0)
                return 0;
            // calculate CRC after reading
            if (crc!=null)
                crc.SlurpBlock(_buf1,0,1);
            return (_buf1[0] & 0xFF);
        }
#endif

        private bool nomoreinput;


        private string ReadZeroTerminatedString()
        {
            var list = new List<byte>();
            var done = false;
            do
            {
                // workitem 7740
                var n = _stream.Read(_buf1, 0, 1);
                if (n != 1)
                    throw new ZlibException("Unexpected EOF reading GZIP header.");

                if (_buf1[0] == 0)
                    done = true;
                else
                    list.Add(_buf1[0]);
            } while (!done);

            var a = list.ToArray();
            return GZipStream.iso8859dash1.GetString(a, 0, a.Length);
        }


        private int ReadAndValidateGzipHeader()
        {
            var totalBytesRead = 0;
            // read the header on the first read
            var header = new byte[10];
            var n = _stream.Read(header, 0, header.Length);

            // workitem 8501: handle edge case (decompress empty stream)
            if (n == 0)
                return 0;

            if (n != 10)
                throw new ZlibException("Not a valid GZIP stream.");

            if (header[0] != 0x1F || header[1] != 0x8B || header[2] != 8)
                throw new ZlibException("Bad GZIP header.");

            var timet = BitConverter.ToInt32(header, 4);
            _GzipMtime = GZipStream._unixEpoch.AddSeconds(timet);
            totalBytesRead += n;
            if ((header[3] & 0x04) == 0x04)
            {
                // read and discard extra field
                n = _stream.Read(header, 0, 2); // 2-byte length field
                totalBytesRead += n;

                var extraLength = (short)(header[0] + header[1] * 256);
                var extra = new byte[extraLength];
                n = _stream.Read(extra, 0, extra.Length);
                if (n != extraLength)
                    throw new ZlibException("Unexpected End-of-file reading GZIP header.");
                totalBytesRead += n;
            }

            if ((header[3] & 0x08) == 0x08)
                _GzipFileName = ReadZeroTerminatedString();
            if ((header[3] & 0x10) == 0x010)
                _GzipComment = ReadZeroTerminatedString();
            if ((header[3] & 0x02) == 0x02)
                Read(_buf1, 0, 1); // CRC16, ignore

            return totalBytesRead;
        }


        public override int Read(byte[] buffer, int offset, int count)
        {
            // According to MS documentation, any implementation of the IO.Stream.Read function must:
            // (a) throw an exception if offset & count reference an invalid part of the buffer,
            //     or if count < 0, or if buffer is null
            // (b) return 0 only upon EOF, or if count = 0
            // (c) if not EOF, then return at least 1 byte, up to <count> bytes

            if (_streamMode == StreamMode.Undefined)
            {
                if (!_stream.CanRead)
                    throw new ZlibException("The stream is not readable.");
                // for the first read, set up some controls.
                _streamMode = StreamMode.Reader;
                // (The first reference to _z goes through the private accessor which
                // may initialize it.)
                Z.AvailableBytesIn = 0;
                if (_flavor == ZlibStreamFlavor.GZIP)
                {
                    _gzipHeaderByteCount = ReadAndValidateGzipHeader();
                    // workitem 8501: handle edge case (decompress empty stream)
                    if (_gzipHeaderByteCount == 0)
                        return 0;
                }
            }

            if (_streamMode != StreamMode.Reader)
                throw new ZlibException("Cannot Read after Writing.");

            if (count == 0)
                return 0;
            if (nomoreinput && WantCompress)
                return 0; // workitem 8557
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (offset < buffer.GetLowerBound(0))
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (offset + count > buffer.GetLength(0))
                throw new ArgumentOutOfRangeException(nameof(count));

            int rc;

            // set up the output of the deflate/inflate codec:
            _z.OutputBuffer = buffer;
            _z.NextOut = offset;
            _z.AvailableBytesOut = count;

            // This is necessary in case _workingBuffer has been resized. (new byte[])
            // (The first reference to _workingBuffer goes through the private accessor which
            // may initialize it.)
            _z.InputBuffer = WorkingBuffer;

            do
            {
                // need data in _workingBuffer in order to deflate/inflate.  Here, we check if we have any.
                if (_z.AvailableBytesIn == 0 && !nomoreinput)
                {
                    // No data available, so try to Read data from the captive stream.
                    _z.NextIn = 0;
                    _z.AvailableBytesIn = _stream.Read(_workingBuffer, 0, _workingBuffer.Length);
                    if (_z.AvailableBytesIn == 0)
                        nomoreinput = true;
                }

                // we have data in InputBuffer; now compress or decompress as appropriate
                rc = WantCompress
                    ? _z.Deflate(_flushMode)
                    : _z.Inflate(_flushMode);

                if (nomoreinput && rc == ZlibConstants.Z_BUF_ERROR)
                    return 0;

                if (rc != ZlibConstants.Z_OK && rc != ZlibConstants.Z_STREAM_END)
                    throw new ZlibException($"{(WantCompress ? "de" : "in")}flating:  rc={rc}  msg={_z.Message}");

                if ((nomoreinput || rc == ZlibConstants.Z_STREAM_END) && _z.AvailableBytesOut == count)
                    break; // nothing more to read
            }
            //while (_z.AvailableBytesOut == count && rc == ZlibConstants.Z_OK);
            while (_z.AvailableBytesOut > 0 && !nomoreinput && rc == ZlibConstants.Z_OK);


            // workitem 8557
            // is there more room in output?
            if (_z.AvailableBytesOut > 0)
            {
                if (rc == ZlibConstants.Z_OK && _z.AvailableBytesIn == 0)
                {
                    // deferred
                }

                // are we completely done reading?
                if (nomoreinput)
                    // and in compression?
                    if (WantCompress)
                    {
                        // no more input data available; therefore we flush to
                        // try to complete the read
                        rc = _z.Deflate(FlushType.Finish);

                        if (rc != ZlibConstants.Z_OK && rc != ZlibConstants.Z_STREAM_END)
                            throw new ZlibException($"Deflating:  rc={rc}  msg={_z.Message}");
                    }
            }


            rc = count - _z.AvailableBytesOut;

            // calculate CRC after reading
            if (crc != null)
                crc.SlurpBlock(buffer, offset, rc);

            return rc;
        }


        public override bool CanRead => _stream.CanRead;

        public override bool CanSeek => _stream.CanSeek;

        public override bool CanWrite => _stream.CanWrite;

        public override long Length => _stream.Length;

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public enum StreamMode
        {
            Writer,
            Reader,
            Undefined
        }


        internal static void CompressString(string s, Stream compressor)
        {
            var uncompressed = Encoding.UTF8.GetBytes(s);
            using (compressor)
            {
                compressor.Write(uncompressed, 0, uncompressed.Length);
            }
        }

        internal static void CompressBuffer(byte[] b, Stream compressor)
        {
            // workitem 8460
            using (compressor)
            {
                compressor.Write(b, 0, b.Length);
            }
        }

        internal static string UncompressString(byte[] compressed, Stream decompressor)
        {
            if (compressed == null)
                throw new ArgumentNullException(nameof(compressed));
            // workitem 8460
            var working = new byte[1024];
            var encoding = Encoding.UTF8;
            using var output = new MemoryStream();
            using (decompressor)
            {
                int n;
                while ((n = decompressor.Read(working, 0, working.Length)) != 0)
                    output.Write(working, 0, n);
            }

            // reset to allow read from start
            output.Seek(0, SeekOrigin.Begin);
            using var sr = new StreamReader(output, encoding);
            return sr.ReadToEnd();
        }

        internal static byte[] UncompressBuffer(byte[] compressed, Stream decompressor)
        {
            if (compressed == null)
                throw new ArgumentNullException(nameof(compressed));
            // workitem 8460
            var working = new byte[1024];
            using var output = new MemoryStream();
            using (decompressor)
            {
                int n;
                while ((n = decompressor.Read(working, 0, working.Length)) != 0)
                    output.Write(working, 0, n);
            }

            return output.ToArray();
        }
    }
}