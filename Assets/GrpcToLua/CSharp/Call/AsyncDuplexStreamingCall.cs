using Grpc.Core;
using System;
using System.Threading.Tasks;

namespace GrpcToLua
{
    /// <summary>
    /// Return type for bidirectional streaming calls.
    /// </summary>
    public sealed class AsyncDuplexStreamingCall : IDisposable
    {
        readonly AsyncDuplexStreamingCall<byte[], byte[]> call;

        /// <summary>
        /// Creates a new AsyncDuplexStreamingCall object.
        /// </summary>
        public AsyncDuplexStreamingCall(AsyncDuplexStreamingCall<byte[], byte[]> call)
        {
            this.call = call;
        }

        /// <summary>
        /// Asynchronous access to response headers.
        /// </summary>
        public Task<Metadata> ResponseHeadersAsync
        {
            get
            {
                return call.ResponseHeadersAsync;
            }
        }

        /// <summary>
        /// Gets the call status if the call has already finished.
        /// Throws InvalidOperationException otherwise.
        /// </summary>
        public Status GetStatus()
        {
            return call.GetStatus();
        }

        /// <summary>
        /// Gets the call trailing metadata if the call has already finished.
        /// Throws InvalidOperationException otherwise.
        /// </summary>
        public Metadata GetTrailers()
        {
            return call.GetTrailers();
        }

        /// <summary>
        /// Provides means to cleanup after the call.
        /// If the call has already finished normally (request stream has been completed and response stream has been fully read), doesn't do anything.
        /// Otherwise, requests cancellation of the call which should terminate all pending async operations associated with the call.
        /// As a result, all resources being used by the call should be released eventually.
        /// </summary>
        /// <remarks>
        /// Normally, there is no need for you to dispose the call unless you want to utilize the
        /// "Cancel" semantics of invoking <c>Dispose</c>.
        /// </remarks>
        public void Dispose()
        {
            call.Dispose();
        }
        public Task WriteAsync(byte[] requestData)
        {
            return call.RequestStream.WriteAsync(requestData);
        }

        public Task CompleteAsync()
        {
            return call.RequestStream.CompleteAsync();
        }

        public async Task<byte[]> GetNextResponseAsync()
        {
            var responseStream = call.ResponseStream;
            bool ok = await responseStream.MoveNext().ConfigureAwait(false);
            if (ok)
            {
                return responseStream.Current;
            }
            return null;
        }
    }
}
