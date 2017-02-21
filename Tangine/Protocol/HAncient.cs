using System;
using System.Text;

namespace Tangine.Protocol
{
    public class HAncient : HResolver
    {
        public HAncient(bool isOutgoing)
            : base(isOutgoing)
        { }

        public override ushort GetHeader(byte[] data)
        {
            throw new NotImplementedException();
        }
        public override int GetBodyLength(byte[] data)
        {
            throw new NotImplementedException();
        }

        public override int ReadInt32(byte[] data, int index)
        {
            int decoded = (data[index] & 3);
            int length = ((data[index] >> 3) & 7);
            bool isNegative = ((data[index] & 4) == 4);
            for (int i = 1, j = (index + 1), k = 2; i < length; i++, j++)
            {
                if (length > (data.Length - index)) break;
                decoded |= ((data[j] & 63) << k);
                k = (2 + (i * 6));
            }
            return (isNegative ? -decoded : decoded);
        }
        public override string ReadUTF8(byte[] data, int index)
        {
            int length = 0;
            if (IsOutgoing)
            {
                length = ReadUInt16(data, index);
                length += 2;
            }
            else
            {
                for (int i = index; i < data.Length; i++)
                {
                    if (data[i] == 2) break;
                    length++;
                }
            }
            return Encoding.UTF8.GetString(data, index, length);
        }
        public override bool ReadBoolean(byte[] data, int index)
        {
            return (data[index] == 73);
        }
        public override ushort ReadUInt16(byte[] data, int index)
        {
            if (data.Length > 1)
            {
                int result = (data[index++] - 64);
                result *= 64;
                result += 64;
                return (ushort)(data[index] - result);
            }
            else return 0;
        }
    }
}