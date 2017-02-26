using System.Threading.Tasks;
using System.Collections.Generic;

using Tangine.Network;

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

        public abstract byte[] GetBody(byte[] data);
        public abstract ushort GetHeader(byte[] data);

        public abstract int GetSize(int value);
        public abstract int GetSize(bool value);
        public abstract int GetSize(string value);
        public abstract int GetSize(ushort value);
        public abstract int GetSize(double value);

        public abstract int ReadInt32(IList<byte> data, int index);
        public abstract string ReadUTF8(IList<byte> data, int index);
        public abstract bool ReadBoolean(IList<byte> data, int index);
        public abstract ushort ReadUInt16(IList<byte> data, int index);
        public abstract double ReadDouble(IList<byte> data, int index);

        public abstract Task<HPacket> ReceivePacketAsync(HNode node);

        public abstract HPacket CreatePacket(byte[] data);
        public abstract HPacket CreatePacket(ushort header, params object[] values);
    }
}