using System.Net;
using System.Threading.Tasks;

using Sulakore.Protocol;
using Sulakore.Communication;

namespace Tangine
{
    internal class ContractorProxy : IHConnection
    {
        private readonly HNode _remoteContractor;

        public IPEndPoint Proxy { get; set; }
        public bool IsUsingProxy { get; set; }
        public HotelEndPoint RemoteEndPoint { get; set; }

        public ContractorProxy(HNode remoteContractor)
        {
            _remoteContractor = remoteContractor;
        }

        public Task<int> SendToServerAsync(byte[] data)
        {
            return _remoteContractor.SendMessageAsync(5, data.Length, data);
        }
        public Task<int> SendToServerAsync(HMessage message)
        {
            return SendToServerAsync(message.ToBytes());
        }
        public Task<int> SendToServerAsync(ushort header, params object[] values)
        {
            return SendToServerAsync(HMessage.Construct(header, values));
        }

        public Task<int> SendToClientAsync(byte[] data)
        {
            return _remoteContractor.SendMessageAsync(4, data.Length, data);
        }
        public Task<int> SendToClientAsync(HMessage message)
        {
            return SendToClientAsync(message.ToBytes());
        }
        public Task<int> SendToClientAsync(ushort header, params object[] values)
        {
            return SendToClientAsync(HMessage.Construct(header, values));
        }
    }
}