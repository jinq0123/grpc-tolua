# Design

* protobuf serialization and deserialization are done on the Lua side.
	+ the rpc request and response type is byte[]
	+ uses lua-protobuf
	
## Why not export grpc Call directly

The stream in streaming call is an interface, which tolua can not export.

Take `AsyncClientStreamingCall` for example:
```
namespace Grpc.Core
{
    public sealed class AsyncClientStreamingCall<TRequest, TResponse> : IDisposable
    {
        public IClientStreamWriter<TRequest> RequestStream
        ...
    }
}
```

The grpc-tolua does not expose the stream of call but provides methods to read and to write.


