using System.Threading.Tasks;

using Tangine.Network.Protocol;

namespace Tangine.Network
{
    public interface IHConnection
    {
        HotelEndPoint RemoteEndPoint { get; }

        Task<int> SendToServerAsync(byte[] data);
        Task<int> SendToServerAsync(HPacket packet);

        Task<int> SendToClientAsync(byte[] data);
        Task<int> SendToClientAsync(HPacket packet);
    }
}