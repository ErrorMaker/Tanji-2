using System.Text;

namespace Tangine.Protocol
{
    public class HModern : HResolver
    {
        public HModern()
            : base(false)
        { }

        public override ushort GetHeader(byte[] data)
        {
            return ReadUInt16(data, 4);
        }
        public override int GetBodyLength(byte[] data)
        {
            return ReadInt32(data, 0);
        }

        public override int ReadInt32(byte[] data, int index)
        {
            int result = (data[index++] << 24);
            result += (data[index++] << 16);
            result += (data[index++] << 8);
            return (result + data[index]);
        }
        public override string ReadUTF8(byte[] data, int index)
        {
            ushort length = ReadUInt16(data, index);
            index += 2;

            return Encoding.UTF8.GetString(data, index, length);
        }
        public override bool ReadBoolean(byte[] data, int index)
        {
            return (data[index] == 1);
        }
        public override ushort ReadUInt16(byte[] data, int index)
        {
            int result = (data[index++] << 8);
            return (ushort)(result + data[index]);
        }
    }
}