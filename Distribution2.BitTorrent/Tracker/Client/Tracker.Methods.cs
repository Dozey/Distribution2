using System;
using System.Net;
using Distribution2.BitTorrent.Tracker.Client.Http;
using Distribution2.BitTorrent.Tracker.Client.Udp;

namespace Distribution2.BitTorrent.Tracker.Client
{
    public partial class Tracker
    {
        #region Announce Requests

        public IAnnounceRequest CreateAnnounceRequest(InfoHash infoHash, PeerId peerId, int port, long uploaded, long downloaded, long left)
        {
            IAnnounceRequest request = CreateAnnounceRequest<IAnnounceRequest>();
            request.InfoHash = infoHash;
            request.PeerId = peerId;
            request.Port = port;
            request.Uploaded = uploaded;
            request.Downloaded = downloaded;
            request.Left = left;

            return request;
        }

        public IAnnounceRequest CreateAnnounceRequest(InfoHash infoHash, PeerId peerId, int port, long uploaded, long downloaded, long left, IPAddress ip)
        {

            IAnnounceRequest request = CreateAnnounceRequest(infoHash, peerId, port, uploaded, downloaded, left);
            request.Ip = ip;

            return request;
        }

        public IAnnounceRequest CreateAnnounceRequest(InfoHash infoHash, PeerId peerId, int port, long uploaded, long downloaded, long left, HttpTrackerEvent clientEvent)
        {
            IAnnounceRequest request = CreateAnnounceRequest(infoHash, peerId, port, uploaded, downloaded, left);
            request.Event = clientEvent;

            return request;
        }

        public IAnnounceRequest CreateAnnounceRequest(InfoHash infoHash, PeerId peerId, int port, long uploaded, long downloaded, long left, IPAddress ip, HttpTrackerEvent clientEvent)
        {
            IAnnounceRequest request = CreateAnnounceRequest(infoHash, peerId, port, uploaded, downloaded, left);
            request.Ip = ip;
            request.Event = clientEvent;

            return request;
        }

        public IAnnounceRequest CreateAnnounceRequest(InfoHash infoHash, PeerId peerId, int port, long uploaded, long downloaded, long left, IPAddress ip, HttpTrackerEvent clientEvent, bool compact)
        {
            IAnnounceRequest2 request = CreateAnnounceRequest<IAnnounceRequest2>();
            request.InfoHash = infoHash;
            request.PeerId = peerId;
            request.Port = port;
            request.Uploaded = uploaded;
            request.Downloaded = downloaded;
            request.Left = left;
            request.Ip = ip;
            request.Event = clientEvent;

            // UdpAnnounceRequest.Compact is read-only
            if (!(request is UdpAnnounceRequest))
            {
                if (!compact)
                    throw new NotSupportedException("UdpAnnounceRequest does not suppor non-compact requests");
            }
            else
            {
                request.Compact = compact;
            }



            return request;
        }

        public IAnnounceRequest CreateAnnounceRequest(InfoHash infoHash, PeerId peerId, int port, long uploaded, long downloaded, long left, IPAddress ip, HttpTrackerEvent clientEvent, int numWant)
        {
            if (Protocol == TrackerProtocol.UDP)
                throw new NotSupportedException("Udp trackers do not support IAnnounceRequest3 specification");

            IAnnounceRequest3 request = CreateAnnounceRequest<IAnnounceRequest3>();
            request.InfoHash = infoHash;
            request.PeerId = peerId;
            request.Port = port;
            request.Uploaded = uploaded;
            request.Downloaded = downloaded;
            request.Left = left;
            request.Ip = ip;
            request.Event = clientEvent;
            request.NumWant = numWant;

            return request;
        }

        public IAnnounceRequest CreateAnnounceRequest(InfoHash infoHash, PeerId peerId, int port, long uploaded, long downloaded, long left, IPAddress ip, HttpTrackerEvent clientEvent, bool compact, int numWant)
        {
            if (Protocol == TrackerProtocol.UDP)
                throw new NotSupportedException("Udp trackers do not support IAnnounceRequest3 specification");

            IAnnounceRequest3 request = CreateAnnounceRequest<IAnnounceRequest3>();
            request.InfoHash = infoHash;
            request.PeerId = peerId;
            request.Port = port;
            request.Uploaded = uploaded;
            request.Downloaded = downloaded;
            request.Left = left;
            request.Ip = ip;
            request.Event = clientEvent;

            // UdpAnnounceRequest.Compact is read-only
            if (!(request is UdpAnnounceRequest))
            {
                if (!compact)
                    throw new NotSupportedException("UdpAnnounceRequest does not suppor non-compact requests");
            }
            else
            {
                request.Compact = compact;
            }

            request.NumWant = numWant;

            return request;
        }

        public IAnnounceRequest CreateAnnounceRequest(InfoHash infoHash, PeerId peerId, int port, long uploaded, long downloaded, long left, IPAddress ip, HttpTrackerEvent clientEvent, bool compact, int numWant, bool noPeerId)
        {
            if (Protocol == TrackerProtocol.UDP)
                throw new NotSupportedException("Udp trackers do not support IAnnounceRequest3 specification");

            IAnnounceRequest3 request = CreateAnnounceRequest<IAnnounceRequest3>();
            request.InfoHash = infoHash;
            request.PeerId = peerId;
            request.Port = port;
            request.Uploaded = uploaded;
            request.Downloaded = downloaded;
            request.Left = left;
            request.Ip = ip;
            request.Event = clientEvent;

            // UdpAnnounceRequest.Compact is read-only
            if (!(request is UdpAnnounceRequest))
            {
                if (!compact)
                    throw new NotSupportedException("UdpAnnounceRequest does not suppor non-compact requests");
            }
            else
            {
                request.Compact = compact;
            }

            request.NumWant = numWant;
            request.NoPeerId = noPeerId;

            return request;
        }

        public IAnnounceRequest CreateAnnounceRequest(InfoHash infoHash, PeerId peerId, int port, long uploaded, long downloaded, long left, IPAddress ip, HttpTrackerEvent clientEvent, bool compact, int numWant, bool noPeerId, string key, string trackerId)
        {
            if (Protocol == TrackerProtocol.UDP)
                throw new NotSupportedException("Udp trackers do not support IAnnounceRequest3 specification");

            IAnnounceRequest3 request = CreateAnnounceRequest<IAnnounceRequest3>();
            request.InfoHash = infoHash;
            request.PeerId = peerId;
            request.Port = port;
            request.Uploaded = uploaded;
            request.Downloaded = downloaded;
            request.Left = left;
            request.Ip = ip;
            request.Event = clientEvent;

            // UdpAnnounceRequest.Compact is read-only
            if (!(request is UdpAnnounceRequest))
            {
                if (!compact)
                    throw new NotSupportedException("UdpAnnounceRequest does not suppor non-compact requests");
            }
            else
            {
                request.Compact = compact;
            }

            request.NumWant = numWant;
            request.NoPeerId = noPeerId;
            request.Key = key;
            request.TrackerId = trackerId;

            return request;
        }

        #endregion

        #region Scrape Requests

        public IScrapeRequest CreateScrapeRequest()
        {
            return CreateScrapeRequest<IScrapeRequest>();
        }

        public IScrapeRequest CreateScrapeRequest(InfoHash infoHash)
        {
            IScrapeRequest request = CreateScrapeRequest();
            request.InfoHash = infoHash;

            return request;
        }

        public IScrapeRequest CreateScrapeRequest(params InfoHash[] infoHash)
        {
            if (infoHash.Length == 1)
            {
                return CreateScrapeRequest(infoHash[0]);
            }

            IScrapeRequest2 request = CreateScrapeRequest<IScrapeRequest2>();
            request.InfoHashList.AddRange(infoHash);

            return request;
        }

        #endregion

        private TRequest CreateAnnounceRequest<TRequest>() where TRequest : class, IAnnounceRequest
        {
            Type requestType = typeof(TRequest);
            TRequest request = null;

            switch (Protocol)
            {
                case TrackerProtocol.HTTP:
                case TrackerProtocol.HTTPS:
                    return new HttpAnnounceRequest(this) as TRequest;
                case TrackerProtocol.UDP:
                    if (requestType == typeof(IAnnounceRequest) || requestType == typeof(IAnnounceRequest2))
                        request = new UdpAnnounceRequest(this) as TRequest;
                    break;
                default:
                    throw new NotSupportedException();
            }

            if (request == null)
                throw new NotSupportedException("Unsupported announce request implementation");

            return request;
        }

        private TRequest CreateScrapeRequest<TRequest>() where TRequest : class, IScrapeRequest
        {
            Type requestType = typeof(TRequest);
            TRequest request = null;

            switch (Protocol)
            {
                case TrackerProtocol.HTTP:
                case TrackerProtocol.HTTPS:
                    return new HttpScrapeRequest(this) as TRequest;
                case TrackerProtocol.UDP:
                    if(requestType == typeof(IScrapeRequest) || requestType == typeof(IScrapeRequest2))
                        return new UdpScrapeRequest(this) as TRequest;
                    break;
            }

            if (request == null)
                throw new NotSupportedException("Unsupported scrape request implementation");

            return request;
        }
    }
}
