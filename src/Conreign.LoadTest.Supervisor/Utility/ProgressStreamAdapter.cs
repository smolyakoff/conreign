using System;
using System.IO;
using System.Threading;

namespace Conreign.LoadTest.Supervisor.Utility
{
    public class ProgressStreamAdapter : Stream
    {
        private readonly Stream _stream;
        private readonly IProgress<ProgressStreamEventArgs> _progress;
        private int _read;
        private int _written;

        public ProgressStreamAdapter(Stream stream, IProgress<ProgressStreamEventArgs> progress)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            _stream = stream;
            _progress = progress;
        }

        public override void Flush()
        {
            _stream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _stream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var read = _stream.Read(buffer, offset, count);
            Interlocked.Add(ref _read, read);
            _progress.Report(new ProgressStreamEventArgs(_written, _read));
            return read;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer, offset, count);
            Interlocked.Add(ref _written, count);
            _progress.Report(new ProgressStreamEventArgs(_written, _read));
        }

        public override bool CanRead => _stream.CanRead;
        public override bool CanSeek => _stream.CanSeek;
        public override bool CanWrite => _stream.CanWrite;
        public override long Length => _stream.Length;

        public override long Position
        {
            get { return _stream.Position; }
            set { _stream.Position = value; }
        }
    }
}
