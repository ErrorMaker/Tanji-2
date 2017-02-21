namespace Tangine.Protocol
{
    public class HResolverFactory
    {
        public HModern Modern { get; }
        public HAncient AncientIn { get; }
        public HAncient AncientOut { get; }

        public HResolverFactory()
        {
            Modern = new HModern();
            AncientIn = new HAncient(false);
            AncientOut = new HAncient(true);
        }
    }
}