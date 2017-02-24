using System.Text;
using System.Collections.Generic;

namespace Tangine.Protocol
{
    public class HAncientResolver : HResolver
    {
        public override int LengthBlockSize => (IsOutgoing ? 3 : 0);

        public HAncientResolver(bool isOutgoing)
            : base(isOutgoing)
        { }

        public override byte[] GetBody(byte[] data)
        {
            return null;
        }
        public override ushort GetHeader(byte[] data)
        {
            return 0;
        }

        public override int GetBodyLength(byte[] data)
        {
            return 0;
        }

        public override int GetSize(int value)
        {
            return 0;
        }
        public override int GetSize(bool value)
        {
            return 0;
        }
        public override int GetSize(string value)
        {
            return 0;
        }
        public override int GetSize(ushort value)
        {
            return 0;
        }
        public override int GetSize(double value)
        {
            return 0;
        }

        public override HPacket CreatePacket(byte[] data)
        {
            return new HAncient(IsOutgoing, data);
        }
        public override HPacket CreatePacket(ushort header, params object[] values)
        {
            return new HAncient(IsOutgoing, header, values);
        }

        public override int ReadInt32(IList<byte> data, int index)
        {
            int decoded = (data[index] & 3);
            int length = ((data[index] >> 3) & 7);
            bool isNegative = ((data[index] & 4) == 4);
            for (int i = 1, j = (index + 1), k = 2; i < length; i++, j++)
            {
                if (length > (data.Count - index)) break;
                decoded |= ((data[j] & 63) << k);
                k = (2 + (i * 6));
            }
            return (isNegative ? -decoded : decoded);
        }
        public override string ReadUTF8(IList<byte> data, int index)
        {
            int length = 0;
            if (IsOutgoing)
            {
                length = ReadUInt16(data, index);
                length += 2;
            }
            else
            {
                for (int i = index; i < data.Count; i++)
                {
                    if (data[i] == 2) break;
                    length++;
                }
            }

            var chunk = new byte[length];
            for (int i = index, j = 0; i < length; i++, j++)
            {
                chunk[j] = data[i];
            }
            return Encoding.UTF8.GetString(chunk, index, length);
        }
        public override bool ReadBoolean(IList<byte> data, int index)
        {
            return (data[index] == 73);
        }
        public override ushort ReadUInt16(IList<byte> data, int index)
        {
            if (data.Count > 1)
            {
                int result = (data[index++] - 64);
                result *= 64;
                result += 64;
                return (ushort)(data[index] - result);
            }
            else return 0;
        }
        public override double ReadDouble(IList<byte> data, int index)
        {
            return double.NaN;
        }
    }
}