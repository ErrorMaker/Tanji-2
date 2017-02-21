namespace Tangine.Protocol
{
    public class HPacket
    {
        private int _position;
        public int Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public ushort Header { get; set; }
        public int Length { get; private set; }

        public int ReadInt32()
        {
            return ReadInt32(ref _position);
        }
        public int ReadInt32(int position)
        {
            return ReadInt32(ref position);
        }
        public int ReadInt32(ref int position)
        {
            return 0;
        }

        public string ReadUTF8()
        {
            return ReadUTF8(ref _position);
        }
        public string ReadUTF8(int position)
        {
            return ReadUTF8(ref position);
        }
        public string ReadUTF8(ref int position)
        {
            return null;
        }

        public bool ReadBoolean()
        {
            return ReadBoolean(ref _position);
        }
        public bool ReadBoolean(int position)
        {
            return ReadBoolean(ref position);
        }
        public bool ReadBoolean(ref int position)
        {
            return false;
        }

        public ushort ReadUInt16()
        {
            return ReadUInt16(ref _position);
        }
        public ushort ReadUInt16(int position)
        {
            return ReadUInt16(ref position);
        }
        public ushort ReadUInt16(ref int position)
        {
            return 0;
        }

        public double ReadDouble()
        {
            return ReadDouble(ref _position);
        }
        public double ReadDouble(int position)
        {
            return ReadDouble(ref position);
        }
        public double ReadDouble(ref int position)
        {
            return double.NaN;
        }

        public byte[] ReadBytes(int length)
        {
            return ReadBytes(length, ref _position);
        }
        public byte[] ReadBytes(int length, int position)
        {
            return ReadBytes(length, ref position);
        }
        public byte[] ReadBytes(int length, ref int position)
        {
            return null;
        }

        public int GetReadableBytes(int position)
        {
            return 0;
        }

        public byte[] ToBytes()
        {
            return null;
        }
    }
}