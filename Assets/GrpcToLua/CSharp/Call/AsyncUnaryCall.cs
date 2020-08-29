using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using grpc = Grpc.Core;

namespace GrpcToLua
{
    /// <summary>
    /// Return type for single request - single response call.
    /// </summary>
    public sealed class AsyncUnaryCall : IDisposable
    {
        readonly grpc::AsyncUnaryCall<byte[]> call;
        
        /// <summary>
        /// Creates a new AsyncUnaryCall object.
        /// </summary>
        public AsyncUnaryCall(grpc::AsyncUnaryCall<byte[]> call)
        {
            this.call = call;
        }

        /// <summary>
        /// Asynchronous call result.
        /// </summary>
        public Task<byte[]> ResponseAsync
        {
            get
            {
                return call.ResponseAsync;
            }
        }

        /// <summary>
        /// Asynchronous access to response headers.
        /// </summary>
        public Task<grpc::Metadata> ResponseHeadersAsync
        {
            get
            {
                return call.ResponseHeadersAsync;
            }
        }

        /// <summary>
        /// Allows awaiting this object directly.
        /// </summary>
        public TaskAwaiter<byte[]> GetAwaiter()
        {
            return call.GetAwaiter();
        }

        /// <summary>
        /// Gets the call status if the call has already finished.
        /// Throws InvalidOperationException otherwise.
        /// </summary>
        public grpc::Status GetStatus()
        {
            return call.GetStatus();
        }

        /// <summary>
        /// Gets the call trailing metadata if the call has already finished.
        /// Throws InvalidOperationException otherwise.
        /// </summary>
        public grpc::Metadata GetTrailers()
        {
            return call.GetTrailers();
        }

        /// <summary>
        /// Provides means to cleanup after the call.
        /// If the call has already finished normally (request stream has been completed and call result has been received), doesn't do anything.
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
    }
}
