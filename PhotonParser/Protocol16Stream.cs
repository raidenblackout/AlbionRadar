using System;
using System.IO;

namespace PhotonParser
{

    public class Protocol16Stream : Stream
    {
        private byte[] _buffer;

        private int _position;

        private int _length;

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => true;

        public override long Length => _length;

        public override long Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = (int)value;
                if (_length < _position)
                {
                    _length = _position;
                    ExpandIfNeeded(_length);
                }
            }
        }

        public int Capacity => _buffer.Length;

        public Protocol16Stream(int size = 0)
        {
            _buffer = new byte[size];
        }

        public Protocol16Stream(byte[] buffer)
        {
            _buffer = buffer;
            _length = buffer.Length;
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int num = _length - _position;
            if (num <= 0)
            {
                return 0;
            }

            if (count > num)
            {
                count = num;
            }

            Buffer.BlockCopy(_buffer, _position, buffer, offset, count);
            _position += count;
            return count;
        }

        public override int ReadByte()
        {
            if (_position >= _length)
            {
                return -1;
            }

            byte result = _buffer[_position];
            _position++;
            return result;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            int num = origin switch
            {
                SeekOrigin.Begin => (int)offset,
                SeekOrigin.Current => _position + (int)offset,
                SeekOrigin.End => _length + (int)offset,
                _ => throw new ArgumentException("Invalid seek origin"),
            };
            if (num < 0)
            {
                throw new ArgumentException("Seek before begin");
            }

            if (num > _length)
            {
                throw new ArgumentException("Seek after end");
            }

            _position = num;
            return _position;
        }

        public override void SetLength(long value)
        {
            _length = (int)value;
            ExpandIfNeeded(_length);
            if (_position > _length)
            {
                _position = _length;
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            int num = _position + count;
            ExpandIfNeeded(num);
            if (num > _length)
            {
                _length = num;
            }

            Buffer.BlockCopy(buffer, offset, _buffer, _position, count);
            _position = num;
        }

        private bool ExpandIfNeeded(long requiredSize)
        {
            if (requiredSize <= Capacity)
            {
                return false;
            }

            int num = Capacity;
            if (num == 0)
            {
                num = 1;
            }

            while (requiredSize > num)
            {
                num *= 2;
            }

            Array.Resize(ref _buffer, num);
            return true;
        }
    }
}