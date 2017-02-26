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

        public int SocketSkip { get; set; } = 1;
        public HNode Local { get; private set; }
        public HNode Remote { get; private set; }
        public bool IsConnected { get; private set; }
        public HotelEndPoint RemoteEndPoint => Remote?.EndPoint;

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
            int interceptCount = 0;
            while (!IsConnected)
            {
                try
                {
                    Local = await HNode.AcceptAsync(endpoint.Port).ConfigureAwait(false);
                    if (++interceptCount == SocketSkip) continue;

                    byte[] buffer = await Local.PeekAsync(6).ConfigureAwait(false);
                    Remote = await HNode.ConnectNewAsync(endpoint).ConfigureAwait(false);
                    if (HResolver.Factory.AncientOut.GetHeader(buffer) == 206)
                    {
                        Local.Resolver = HResolver.Factory.AncientOut;

                        HResolver.Factory.AncientIn.DataBacklog.Clear();
                        Remote.Resolver = HResolver.Factory.AncientIn;
                    }
                    else if (HResolver.Factory.Modern.GetHeader(buffer) == 4000)
                    {
                        Local.Resolver = HResolver.Factory.Modern;
                        Remote.Resolver = HResolver.Factory.Modern;
                    }
                    else
                    {
                        buffer = await Local.ReceiveAsync(512).ConfigureAwait(false);
                        await Remote.SendAsync(buffer).ConfigureAwait(false);

                        buffer = await Remote.ReceiveAsync(1024).ConfigureAwait(false);
                        await Local.SendAsync(buffer).ConfigureAwait(false);

                        await Local.SendAsync(new byte[0]).ConfigureAwait(false);
                        continue;
                    }

                    IsConnected = true;
                    OnConnected(EventArgs.Empty);

                    _inSteps = 0;
                    _outSteps = 0;
                    Task interceptOutgoingTask = InterceptOutgoingAsync();
                    Task interceptIncomingTask = InterceptIncomingAsync();
                }
                finally
                {
                    if (!IsConnected)
                    {
                        Local?.Dispose();
                        Remote?.Dispose();
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