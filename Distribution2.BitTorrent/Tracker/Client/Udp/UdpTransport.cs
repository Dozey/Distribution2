using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MiscUtil.Conversion;
using System.IO;

namespace Distribution2.BitTorrent.Tracker.Client.Udp
{
    abstract class UdpTransport
    {
        private const int transmitTimeout = 15000;
        private IPEndPoint remoteEndPoint;

        internal UdpTransport(IPAddress address, int port, int timeout)
        {
            remoteEndPoint = new IPEndPoint(address, port);
            Timeout = timeout;

            Socket = new Socket(address.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            Socket.DontFragment = true;
            Socket.SendTimeout = transmitTimeout;
            Socket.ReceiveTimeout = transmitTimeout;
            Socket.Connect(remoteEndPoint);
        }

        public Socket Socket { get; private set; }
        public IPAddress RemoteAddress { get; private set;}
        public int Timeout { get; private set;}

        protected void Send<T>(T datagram) where T : struct, ISerializeTransmit
        {
            WaitHandle continueEvent = new ManualResetEvent(false);
            for (int i = 0; i < Timeout; i += 15)
            {
                try
                {
                    byte[] buffer = UdpTrackerPacketExchanger.Exchange<T>(datagram);
                    int count = Socket.SendTo(buffer, remoteEndPoint);

                    if (count != buffer.Length)
                        throw new Exception(String.Format("Failed to send {0}", typeof(T).Name));
                }
                catch
                {
                    continue;
                }

                return;
            }

            throw new TimeoutException(String.Format("Timed out sending {0}", typeof(T).Name));
        }

        protected TResponse Receive<TResponse>() where TResponse : struct, ISerializeReceive
        {
            return Receive<TResponse>(new Predicate<TResponse>(response => true));
        }

        protected TResponse Receive<TResponse>(Predicate<TResponse> filter) where TResponse : struct, ISerializeReceive
        {
            return Receive<TResponse>(filter, null);
        }

        protected TResponse Receive<TResponse>(Predicate<TResponse> filter, Action retransmit) where TResponse : struct, ISerializeReceive
        {
            WaitHandle continueEvent = new ManualResetEvent(false);
            for (int i = 0; i < Timeout; i += 15)
            {
                byte[] buffer = null;
                int count = 0;

                try
                {
                    byte[] tmpBuffer = new byte[1500];
                    EndPoint tmpRemoteEndpoint = (EndPoint)remoteEndPoint;
                    count = Socket.ReceiveFrom(tmpBuffer, ref tmpRemoteEndpoint);
                    buffer = new byte[count];
                    Array.ConstrainedCopy(tmpBuffer, 0, buffer, 0, count);
                }
                catch
                {
                    goto RESET;
                }

                TResponse packet = UdpTrackerPacketExchanger.Exchange<TResponse>(buffer);

                if (filter(packet))
                    return packet;

                continue;

            RESET:
                if(retransmit != null)
                    retransmit();

                continue;
            }

            throw new TimeoutException(String.Format("Timed out receiving {0}", typeof(TResponse).Name));
        }


        private void TransmissionCompleted(IAsyncResult result)
        {
            ((ManualResetEvent)result.AsyncState).Set();
        }

        protected class UdpTrackerPacketExchanger
        {
            private enum PacketType
            {
                ConnectResponse = 0,
                AnnounceResponse = 1,
                ScrapeResponse = 2,
                ErrorResponse = 3,
            }

            public static byte[] Exchange<T>(T packet) where T : struct, ISerializeTransmit
            {
                return packet.ToByteArray();
            }

            public static T Exchange<T>(byte[] packet) where T : struct, ISerializeReceive
            {
                Type packetType = GetPacketType(ref packet);
                
                if(packetType == typeof(T))
                {
                    T responsePacket = new T();
                    responsePacket.FromByteArray(ref packet);

                    return responsePacket;
                }
                else if (packetType == typeof(UdpErrorResponsePacket))
                {
                    UdpErrorResponsePacket errorResponse = new UdpErrorResponsePacket();
                    errorResponse.FromByteArray(ref packet);

                    throw new TrackerFailureException(errorResponse.error);
                }
                else
                {
                    throw new ArgumentException(String.Format("Packet is not '{0}'", packetType.Name));
                }
            }

            private static Type GetPacketType(ref byte[] packet)
            {
                BigEndianBitConverter ec = new BigEndianBitConverter();
                int packetType = ec.ToInt32(packet, 0);

                switch ((PacketType)packetType)
                {
                    case PacketType.AnnounceResponse:
                        return typeof(UdpAnnounceResponsePacket);
                    case PacketType.ConnectResponse:
                        return typeof(UdpConnectResponsePacket);
                    case PacketType.ErrorResponse:
                        return typeof(UdpErrorResponsePacket);
                    case PacketType.ScrapeResponse:
                        return typeof(UdpScrapeResponsePacket);
                    default:
                        throw new NotSupportedException("Unsupported packet");
                }
            }
        }
    }
}
