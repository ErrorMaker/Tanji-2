using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangine.Protocol
{
    public class HModernResolver : HResolver
    {
        public override int LengthBlockSize => 4;

        public HModernResolver()
            : base(false)
        { }

        public override byte[] GetBody(byte[] data)
        {
            var body = new byte[data.Length - 6];
            Buffer.BlockCopy(data, 6, body, 0, body.Length);
            return body;
        }
        public override ushort GetHeader(byte[] data)
        {
            return ReadUInt16(data, 4);
        }

        public override int GetBodyLength(byte[] data)
        {
            return ReadInt32(data, 0);
        }

        public override int GetSize(int value)
        {
            return 4;
        }
        public override int GetSize(bool value)
        {
            return 1;
        }
        public override int GetSize(string value)
        {
            return (2 + Encoding.UTF8.GetByteCount(value));
        }
        public override int GetSize(ushort value)
        {
            return 2;
        }
        public override int GetSize(double value)
        {
            return 6;
        }

        public override HPacket CreatePacket(byte[] data)
        {
            return new HModern(data);
        }
        public override HPacket CreatePacket(ushort header, params object[] values)
        {
            return new HModern(header, values);
        }

        public override int ReadInt32(IList<byte> data, int index)
        {
            int result = (data[index++] << 24);
            result += (data[index++] << 16);
            result += (data[index++] << 8);
            return (result + data[index]);
        }
        public override string ReadUTF8(IList<byte> data, int index)
        {
            ushort length = ReadUInt16(data, index);
            index += 2;

            var chunk = new byte[length];
            for (int i = index, j = 0; j < length; i++, j++)
            {
                chunk[j] = data[i];
            }
            return Encoding.UTF8.GetString(chunk, 0, length);
        }
        public override bool ReadBoolean(IList<byte> data, int index)
        {
            return (data[index] == 1);
        }
        public override ushort ReadUInt16(IList<byte> data, int index)
        {
            int result = (data[index++] << 8);
            return (ushort)(result + data[index]);
        }
        public override double ReadDouble(IList<byte> data, int index)
        {
            var chunk = new byte[data.Count - index];
            for (int i = chunk.Length - 1, j = index + chunk.Length; i >= 0; i--, j--)
            {
                chunk[i] = data[i];
            }
            return BitConverter.ToDouble(chunk, 0);
        }
    }
}