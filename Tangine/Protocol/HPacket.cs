using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Tangine.Protocol
{
    [DebuggerDisplay("Header: {Header} | {ToString()}")]
    public abstract class HPacket
    {
        private byte[] _toBytesCache;
        private string _toStringCache;

        private int _position;
        public int Position
        {
            get { return _position; }
            set { _position = value; }
        }

        private ushort _header;
        public ushort Header
        {
            get { return _header; }
            set
            {
                if (_header != value)
                {
                    _header = value;
                    ResetCache();
                }
            }
        }

        public List<byte> Body { get; }
        public HResolver Resolver { get; }
        public int ReadableBytes => GetReadableBytes(Position);

        public HPacket(byte[] data, HResolver resolver)
        {
            Resolver = resolver;
            Body = new List<byte>();
            Header = resolver.GetHeader(data);
            Body.AddRange(resolver.GetBody(data));

            // We don't want to use original the byte array reference, since it could change outside of the class scope.
            _toBytesCache = new byte[data.Length];
            Buffer.BlockCopy(data, 0, _toBytesCache, 0, data.Length);
        }

        public int ReadInt32()
        {
            return ReadInt32(ref _position);
        }
        public int ReadInt32(int position)
        {
            return ReadInt32(ref position);
        }
        public virtual int ReadInt32(ref int position)
        {
            int value = Resolver.ReadInt32(Body, position);
            position += Resolver.GetSize(value);
            return value;
        }

        public string ReadUTF8()
        {
            return ReadUTF8(ref _position);
        }
        public string ReadUTF8(int position)
        {
            return ReadUTF8(ref position);
        }
        public virtual string ReadUTF8(ref int position)
        {
            string value = Resolver.ReadUTF8(Body, position);
            position += Resolver.GetSize(value);
            return value;
        }

        public bool ReadBoolean()
        {
            return ReadBoolean(ref _position);
        }
        public bool ReadBoolean(int position)
        {
            return ReadBoolean(ref position);
        }
        public virtual bool ReadBoolean(ref int position)
        {
            bool value = Resolver.ReadBoolean(Body, position);
            position += Resolver.GetSize(value);
            return value;
        }

        public ushort ReadUInt16()
        {
            return ReadUInt16(ref _position);
        }
        public ushort ReadUInt16(int position)
        {
            return ReadUInt16(ref position);
        }
        public virtual ushort ReadUInt16(ref int position)
        {
            ushort value = Resolver.ReadUInt16(Body, position);
            position += Resolver.GetSize(value);
            return value;
        }

        public double ReadDouble()
        {
            return ReadDouble(ref _position);
        }
        public double ReadDouble(int position)
        {
            return ReadDouble(ref position);
        }
        public virtual double ReadDouble(ref int position)
        {
            double value = Resolver.ReadDouble(Body, position);
            position += Resolver.GetSize(value);
            return value;
        }

        public byte ReadByte()
        {
            return ReadByte(ref _position);
        }
        public byte ReadByte(int position)
        {
            return ReadByte(ref position);
        }
        public virtual byte ReadByte(ref int position)
        {
            return Body[position++];
        }

        public byte[] ReadBytes(int length)
        {
            return ReadBytes(length, ref _position);
        }
        public byte[] ReadBytes(int length, int position)
        {
            return ReadBytes(length, ref position);
        }
        public virtual byte[] ReadBytes(int length, ref int position)
        {
            var chunk = new byte[length];
            for (int i = 0; i < length; i++)
            {
                chunk[i] = Body[position++];
            }
            return chunk;
        }

        public byte[] ToBytes()
        {
            if (_toBytesCache != null)
            {
                return _toBytesCache;
            }
            return (_toBytesCache = AsBytes());
        }
        public override string ToString()
        {
            if (_toStringCache != null)
            {
                return _toStringCache;
            }
            return (_toStringCache = AsString());
        }
        public int GetReadableBytes(int position)
        {
            return (Body.Count - position);
        }

        private void ResetCache()
        {
            _toBytesCache = null;
            _toStringCache = null;
        }
        protected abstract byte[] AsBytes();
        protected virtual string AsString()
        {
            string result = Encoding.Default.GetString(ToBytes());
            for (int i = 0; i <= 13; i++)
            {
                result = result.Replace(
                    ((char)i).ToString(), ("[" + i + "]"));
            }
            return result;
        }

        public static byte[] Construct(HResolver resolver, ushort header, params object[] values)
        {
            return null;
        }
    }
}