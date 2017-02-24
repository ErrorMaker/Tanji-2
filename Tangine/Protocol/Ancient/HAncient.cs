namespace Tangine.Protocol
{
    public class HAncient : HPacket
    {
        public HAncient(bool isOutgoing, byte[] data)
            : base(data, isOutgoing ? HResolver.Factory.AncientOut : HResolver.Factory.AncientIn)
        { }
        public HAncient(bool isOutgoing, ushort header, params object[] values)
            : this(isOutgoing, Construct(isOutgoing, header, values))
        { }

        protected override byte[] AsBytes()
        {
            return null;
        }
        protected override string AsString()
        {
            return null;
        }

        public static byte[] Construct(bool isOutgoing, ushort header, params object[] values)
        {
            var resolver = (isOutgoing ?
                HResolver.Factory.AncientOut :
                HResolver.Factory.AncientIn);

            return Construct(resolver, header, values);
        }
    }
}