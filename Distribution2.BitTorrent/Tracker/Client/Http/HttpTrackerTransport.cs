using System.IO;

namespace Distribution2.BitTorrent.Tracker.Client.Http
{
    public abstract class HttpTrackerTransport
    {
        protected static Stream BufferResponseStream(Stream source)
        {
            byte[] buffer = new byte[1024];
            MemoryStream bufferStream = new MemoryStream(buffer.Length);

            using (source)
            {
                int bytesRead = -1;

                while (bytesRead != 0)
                {
                    try
                    {
                        bytesRead = source.Read(buffer, 0, buffer.Length);
                        bufferStream.Write(buffer, 0, bytesRead);
                    }
                    catch (System.Net.WebException e)
                    {
                        throw;
                    }
                }

                bufferStream.Flush();
            }

            bufferStream.Seek(0, SeekOrigin.Begin);

            return bufferStream;
        }
    }
}
