using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Tangine.Network;
using Tangine.Protocol;

namespace Tanji.Network
{
    public class HConnection : IHConnection, IDisposable
    {
        private int _inSteps, _outSteps;
        private readonly object _disposeLock;
        private readonly object _disconnectLock;

        /// <summary>
        /// Occurs when the connection between the game client, and server have been intercepted.
        /// </summary>
        public event EventHandler Connected;
        protected virtual void OnConnected(EventArgs e)
        {
            Connected?.Invoke(this, e);
        }

        /// <summary>
        /// Occurs when either the game client, or server have disconnected.
        /// </summary>
        public event EventHandler Disconnected;
        protected virtual void OnDisconnected(EventArgs e)
        {
            Disconnected?.Invoke(this, e);
        }

        /// <summary>
        /// Occurs when the game client's outgoing data has been intercepted.
        /// </summary>
        public event EventHandler<DataInterceptedEventArgs> DataOutgoing;
        protected virtual void OnDataOutgoing(DataInterceptedEventArgs e)
        {
            DataOutgoing?.Invoke(this, e);
        }

        /// <summary>
        /// Occrus when the server's incoming data has been intercepted.
        /// </summary>
        public event EventHandler<DataInterceptedEventArgs> DataIncoming;
        protected virtual void OnDataIncoming(DataInterceptedEventArgs e)
        {
            DataIncoming?.Invoke(this, e);
        }

        private HotelEndPoint _remoteEndPoint;
        public HotelEndPoint RemoteEndPoint
        {
            get
            {
                return (Remote?.EndPoint ?? _remoteEndPoint);
            }
        }

        public HNode Local { get; private set; }
        public HNode Remote { get; private set; }
        public bool IsConnected { get; private set; }

        public HConnection()
        {
            _disposeLock = new object();
            _disconnectLock = new object();
        }

        public Task InterceptAsync(IPEndPoint endpoint)
        {
            return InterceptAsync(new HotelEndPoint(endpoint));
        }
        public Task InterceptAsync(string host, int port)
        {
            return InterceptAsync(HotelEndPoint.Parse(host, port));
        }
        public async Task InterceptAsync(HotelEndPoint endpoint)
        {
            _remoteEndPoint = endpoint;
            int interceptCount = 0;
            while (!IsConnected)
            {
                HNode tempLocal = null;
                HNode tempRemote = null;
                try
                {
                    tempLocal = await HNode.AcceptAsync(endpoint.Port)
                        .ConfigureAwait(false);

                    if (++interceptCount == 2)
                    {
                        interceptCount = 0;
                        continue;
                    }
                    byte[] buffer = await tempLocal.PeekBufferAsync(6)
                        .ConfigureAwait(false);

                    tempRemote = await HNode.ConnectNewAsync(endpoint)
                        .ConfigureAwait(false);

                    if (HResolver.Factory.AncientOut.GetHeader(buffer) == 206)
                    {
                        tempLocal.Resolver = HResolver.Factory.AncientOut;
                        tempRemote.Resolver = HResolver.Factory.AncientIn;
                    }
                    else if (HResolver.Factory.Modern.GetHeader(buffer) == 4000)
                    {
                        tempLocal.Resolver = HResolver.Factory.Modern;
                        tempRemote.Resolver = HResolver.Factory.Modern;
                    }
                    else
                    {
                        buffer = await tempLocal.ReceiveBufferAsync(32).ConfigureAwait(false);
                        await tempRemote.SendAsync(buffer).ConfigureAwait(false);

                        buffer = await tempRemote.ReceiveBufferAsync(1024).ConfigureAwait(false);
                        await tempLocal.SendAsync(buffer).ConfigureAwait(false);

                        continue;
                    }

                    Local = tempLocal;
                    Remote = tempRemote;

                    IsConnected = true;
                    OnConnected(EventArgs.Empty);

                    _outSteps = 0;
                    Task interceptOutgoingTask = InterceptOutgoingAsync();

                    _inSteps = 0;
                    Task interceptIncomingTask = InterceptIncomingAsync();
                }
                finally
                {
                    if (Local != tempLocal)
                    {
                        tempLocal?.Dispose();
                    }
                    if (Remote != tempRemote)
                    {
                        tempRemote?.Dispose();
                    }
                }
            }
        }

        public Task<int> SendToServerAsync(byte[] data)
        {
            return Remote.SendAsync(data);
        }
        public Task<int> SendToServerAsync(HPacket packet)
        {
            return Remote.SendAsync(packet.ToBytes());
        }

        public Task<int> SendToClientAsync(byte[] data)
        {
            return Local.SendAsync(data);
        }
        public Task<int> SendToClientAsync(HPacket packet)
        {
            return Local.SendAsync(packet.ToBytes());
        }

        private async Task InterceptOutgoingAsync()
        {
            HPacket packet = await Local.ReceivePacketAsync().ConfigureAwait(false);
            if (packet != null)
            {
                var args = new DataInterceptedEventArgs(packet, ++_outSteps,
                    true, InterceptOutgoingAsync, SendToServerAsync);

                try { OnDataOutgoing(args); }
                catch { args.Restore(); }

                if (!args.IsBlocked && !args.WasRelayed)
                {
                    await SendToServerAsync(args.Packet)
                        .ConfigureAwait(false);
                }
                if (!args.HasContinued)
                {
                    args.Continue();
                }
            }
            else Disconnect();
        }
        private async Task InterceptIncomingAsync()
        {
            HPacket packet = await Remote.ReceivePacketAsync().ConfigureAwait(false);
            if (packet != null)
            {
                var args = new DataInterceptedEventArgs(packet,
                    ++_inSteps, false, InterceptIncomingAsync, SendToClientAsync);

                try { OnDataIncoming(args); }
                catch { args.Restore(); }

                if (!args.IsBlocked && !args.WasRelayed)
                {
                    await SendToClientAsync(args.Packet)
                        .ConfigureAwait(false);
                }
                if (!args.HasContinued)
                {
                    args.Continue();
                }
            }
            else Disconnect();
        }

        public void Disconnect()
        {
            if (Monitor.TryEnter(_disconnectLock))
            {
                try
                {
                    if (Local != null)
                    {
                        Local.Dispose();
                        Local = null;
                    }
                    if (Remote != null)
                    {
                        Remote.Dispose();
                        Remote = null;
                    }
                    if (IsConnected)
                    {
                        IsConnected = false;
                        OnDisconnected(EventArgs.Empty);
                    }
                }
                finally { Monitor.Exit(_disconnectLock); }
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Disconnect();
            }
        }
    }
}