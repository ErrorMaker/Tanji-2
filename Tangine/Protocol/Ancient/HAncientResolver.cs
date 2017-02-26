using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using Tangine.Network;

namespace Tangine.Protocol
{
    public class HAncientResolver : HResolver
    {
        public List<byte> DataBacklog { get; }

        public HAncientResolver(bool isOutgoing)
            : base(isOutgoing)
        {
            if (!IsOutgoing)
            {
                DataBacklog = new List<byte>(1024);
            }
        }

        public override byte[] GetBody(byte[] data)
        {
            byte[] body = null;
            if (IsOutgoing)
            {
                body = new byte[data.Length - 5];
                Buffer.BlockCopy(data, 5, body, 0, body.Length);
            }
            else
            {
                body = new byte[data.Length - 3];
                Buffer.BlockCopy(data, 2, body, 0, body.Length);
            }
            return body;
        }
        public override ushort GetHeader(byte[] data)
        {
            return ReadUInt16(data, IsOutgoing ? 3 : 0);
        }

        public override int GetSize(int value)
        {
            return 0;
        }
        public override int GetSize(bool value)
        {
            return 0;
        }
        public override int GetSize(string value)
        {
            return 0;
        }
        public override int GetSize(ushort value)
        {
            return 0;
        }
        public override int GetSize(double value)
        {
            return 0;
        }

        public override int ReadInt32(IList<byte> data, int index)
        {
            int decoded = (data[index] & 3);
            int length = ((data[index] >> 3) & 7);
            bool isNegative = ((data[index] & 4) == 4);
            for (int i = 1, j = (index + 1), k = 2; i < length; i++, j++)
            {
                if (length > (data.Count - index)) break;
                decoded |= ((data[j] & 63) << k);
                k = (2 + (i * 6));
            }
            return (isNegative ? -decoded : decoded);
        }
        public override string ReadUTF8(IList<byte> data, int index)
        {
            int length = 0;
            if (IsOutgoing)
            {
                length = ReadUInt16(data, index);
                length += 2;
            }
            else
            {
                for (int i = index; i < data.Count; i++)
                {
                    if (data[i] == 2) break;
                    length++;
                }
            }

            var chunk = new byte[length];
            for (int i = index, j = 0; i < length; i++, j++)
            {
                chunk[j] = data[i];
            }
            return Encoding.UTF8.GetString(chunk, index, length);
        }
        public override bool ReadBoolean(IList<byte> data, int index)
        {
            return (data[index] == 73);
        }
        public override ushort ReadUInt16(IList<byte> data, int index)
        {
            if (data.Count >= 2 && (index + 2) <= data.Count)
            {
                int result = (data[index] - 64);
                result *= 64;
                result += data[index + 1];
                result -= 64;
                return (ushort)result;
            }
            return 0;
        }
        public override double ReadDouble(IList<byte> data, int index)
        {
            return double.NaN;
        }

        public override async Task<HPacket> ReceivePacketAsync(HNode node)
        {
            byte[] data = null;
            if (IsOutgoing)
            {
                byte[] lengthBlock = await node.ReceiveAsync(3).ConfigureAwait(false);
                if (lengthBlock.Length != 3)
                {
                    node.Disconnect();
                    return null;
                }

                int totalBytesRead = 0;
                int nullBytesReadCount = 0;
                var body = new byte[ReadUInt16(lengthBlock, 1)];
                do
                {
                    int bytesLeft = (body.Length - totalBytesRead);
                    int bytesRead = await node.ReceiveAsync(body, totalBytesRead, bytesLeft).ConfigureAwait(false);

                    if (!node.IsConnected || (bytesRead == 0 && ++nullBytesReadCount >= 2))
                    {
                        node.Disconnect();
                        return null;
                    }

                    nullBytesReadCount = 0;
                    totalBytesRead += bytesRead;
                }
                while (totalBytesRead != body.Length);

                data = new byte[3 + body.Length];
                Buffer.BlockCopy(lengthBlock, 0, data, 0, 3);
                Buffer.BlockCopy(body, 0, data, 3, body.Length);
            }
            else
            {
                int nullBytesReadCount = 0;
                data = AttemptStitchBuffer();
                if (data == null)
                {
                    byte[] headerBlock = await node.PeekAsync(2).ConfigureAwait(false);
                    if (headerBlock.Length != 2)
                    {
                        node.Disconnect();
                        return null;
                    }

                    do
                    {
                        byte[] block = await node.ReceiveAsync(256).ConfigureAwait(false);
                        if (!node.IsConnected || (block.Length == 0 && ++nullBytesReadCount >= 2))
                        {
                            node.Disconnect();
                            return null;
                        }

                        nullBytesReadCount = 0;
                        DataBacklog.AddRange(block);

                        data = AttemptStitchBuffer();
                    }
                    while (data == null);
                }
            }
            return CreatePacket(data);
        }

        public override HPacket CreatePacket(byte[] data)
        {
            return new HAncient(IsOutgoing, data);
        }
        public override HPacket CreatePacket(ushort header, params object[] values)
        {
            return new HAncient(IsOutgoing, header, values);
        }

        private byte[] AttemptStitchBuffer()
        {
            byte[] data = null;
            int blockEndIndex = DataBacklog.IndexOf(1);
            if (blockEndIndex != -1)
            {
                int length = (blockEndIndex + 1);
                data = new byte[length];

                DataBacklog.CopyTo(0, data, 0, length);
                DataBacklog.RemoveRange(0, length);
            }
            return data;
        }
    }
}