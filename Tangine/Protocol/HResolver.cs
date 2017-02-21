namespace Tangine.Protocol
{
    public abstract class HResolver
    {
        protected bool IsOutgoing { get; }

        public static HResolverFactory Factory { get; }

        static HResolver()
        {
            Factory = new HResolverFactory();
        }
        protected HResolver()
        { }
        protected HResolver(bool isOutgoing)
        {
            IsOutgoing = isOutgoing;
        }

        public abstract ushort GetHeader(byte[] data);
        public abstract int GetBodyLength(byte[] data);

        public abstract int ReadInt32(byte[] data, int index);
        public abstract string ReadUTF8(byte[] data, int index);
        public abstract bool ReadBoolean(byte[] data, int index);
        public abstract ushort ReadUInt16(byte[] data, int index);
    }
}