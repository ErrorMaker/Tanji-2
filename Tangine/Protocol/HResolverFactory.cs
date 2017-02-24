namespace Tangine.Protocol
{
    public class HResolverFactory
    {
        public HModernResolver Modern { get; }
        public HAncientResolver AncientIn { get; }
        public HAncientResolver AncientOut { get; }

        public HResolverFactory()
        {
            Modern = new HModernResolver();
            AncientIn = new HAncientResolver(false);
            AncientOut = new HAncientResolver(true);
        }
    }
}