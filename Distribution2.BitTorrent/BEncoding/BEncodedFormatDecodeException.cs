using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Distribution2.BitTorrent.BEncoding
{
    public class BEncodedFormatDecodeException : Exception
    {
        private const int TRACE_SEEK_BACK = 25;
        private const int TRACE_SEEK_FORWARD = 25;
        private string decodeTrace;
        private IBEncodedValue targetValue;

        public BEncodedFormatDecodeException(string message, Exception innerException) : base(message, innerException) { }

        public BEncodedFormatDecodeException(string message) : base(message) { }

        public BEncodedFormatDecodeException() : base() { }

        public override string StackTrace
        {
            get { return decodeTrace + base.StackTrace; }
        }

        internal static BEncodedFormatDecodeException CreateTraced(Stream traceStream)
        {
            return CreateTraced(String.Empty, null, traceStream);
        }

        internal static BEncodedFormatDecodeException CreateTraced(string message, Stream traceStream)
        {
            return CreateTraced(message, null, traceStream);
        }

        internal static BEncodedFormatDecodeException CreateTraced(string message, Exception innerException, Stream traceStream)
        {
            StringBuilder traceBuilder = new StringBuilder();
            BEncodedFormatDecodeException exception = new BEncodedFormatDecodeException(message, innerException);

            long seekBack = traceStream.Position > TRACE_SEEK_BACK ? TRACE_SEEK_BACK : traceStream.Position;
            long seekForward = traceStream.Length > (traceStream.Position + TRACE_SEEK_FORWARD) ? TRACE_SEEK_FORWARD : traceStream.Length - traceStream.Position;

            byte[] traceBuffer = new byte[seekBack + seekForward];

            // Back
            traceStream.Seek(seekBack * -1, SeekOrigin.Current);
            traceStream.Read(traceBuffer, 0, (int)seekBack);

            // Forward
            traceStream.Read(traceBuffer, (int)seekBack, (int)seekForward);
            traceStream.Seek(seekForward * -1, SeekOrigin.Current);

            traceBuilder.Append(Encoding.UTF8.GetString(traceBuffer));
            traceBuilder.AppendLine();

            for (int i = 0; i < traceBuffer.Length; i++)
                traceBuilder.Append(i != seekBack ? '-' : '^');

            traceBuilder.Append("    (^ denotes error)");
            traceBuilder.AppendLine();
            traceBuilder.AppendLine();

            exception.decodeTrace = traceBuilder.ToString();   

            return exception;
        }
    }
}
