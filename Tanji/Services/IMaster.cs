using Tangine.Habbo;
using Tangine.Modules;

using Sulakore.Modules;
using Sulakore.Communication;

namespace Tanji.Services
{
    public interface IMaster : IContractor, ITContext, IHaltable
    {
        new HGame Game { get; set; }
        new HConnection Connection { get; }

        void AddReceiver(IReceiver receiver);
        void AddHaltable(IHaltable haltable);
    }
}