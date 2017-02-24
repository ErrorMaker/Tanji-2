using System;

namespace Tangine.Protocol
{
    public class HModern : HPacket
    {
        public HModern(byte[] data)
            : base(data, HResolver.Factory.Modern)
        { }
        public HModern(ushort header, params object[] values)
            : this(Construct(header, values))
        { }

        protected override byte[] AsBytes()
        {
            return null;
        }

        public static byte[] Construct(ushort header, params object[] values)
        {
            return Construct(HResolver.Factory.Modern, header, values);
        }
    }
}