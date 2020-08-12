using LuaInterface;
using UnityEngine;
using grpc = global::Grpc.Core;

namespace GrpcToLua
{
    public class Client : grpc::ClientBase<Client>
    {
        readonly string serviceName;
        MethodDictionary unaryMethods;
        MethodDictionary clientStreamingMethods;
        MethodDictionary serverStreamingMethods;
        MethodDictionary duplexStreamingMethods;

        /// <summary>Creates a new client</summary>
        /// <param name="channel">The channel to use to make remote calls.</param>
        public Client(grpc::ChannelBase channel, string serviceName) : base(channel)
        {
            this.serviceName = serviceName;
            InitMethodDictionaries();
        }

        /// <summary>Creates a new client that uses a custom <c>CallInvoker</c>.</summary>
        /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
        public Client(grpc::CallInvoker callInvoker, string serviceName) : base(callInvoker)
        {
            this.serviceName = serviceName;
            InitMethodDictionaries();
        }

        /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
        /* protected Client() : base()
        {
        }*/

        /// <summary>Protected constructor to allow creation of configured clients.</summary>
        /// <param name="configuration">The client configuration.</param>
        protected Client(ClientBaseConfiguration configuration, string serviceName) : base(configuration)
        {
            this.serviceName = serviceName;
            InitMethodDictionaries();
        }

        private void InitMethodDictionaries()
        {
            unaryMethods = new MethodDictionary(serviceName, grpc.MethodType.Unary);
            clientStreamingMethods = new MethodDictionary(serviceName, grpc.MethodType.ClientStreaming);
            serverStreamingMethods = new MethodDictionary(serviceName, grpc.MethodType.ServerStreaming);
            duplexStreamingMethods = new MethodDictionary(serviceName, grpc.MethodType.DuplexStreaming);
        }

        public grpc::AsyncUnaryCall<LuaTable> UnaryCall(string methodName, LuaTable request)
        {
            Debug.LogFormat("Client.UnaryCall(methodNaame={0}, request={1})", methodName, request);

            var method = unaryMethods.GetMethod(methodName);
            // TODO: input headers, deadline, cancellationToken
            grpc::Metadata headers = null;
            global::System.DateTime? deadline = null;
            global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken);
            var options = new grpc::CallOptions(headers, deadline, cancellationToken);
            return CallInvoker.AsyncUnaryCall(method, null, options, request);
        }
        
        public ServerStreamingCall ServerStreamingCall(string methodName, LuaTable request)
        {
            Debug.LogFormat("Client.ServerStreamingCall(methodNaame={0}, request={1})", methodName, request);
            // TODO
            return new ServerStreamingCall();
        }
        
        public ClientStreamingCall ClientStreamingCall(string methodName, LuaTable request)
        {
            Debug.LogFormat("Client.ClientStreamingCall(methodNaame={0}, request={1})", methodName, request);
            // TODO
            return new ClientStreamingCall();
        }
        
        public DuplexStreamingCall DuplexStreamingCall(string methodName, LuaTable request)
        {
            Debug.LogFormat("Client.DuplexStreamingCall(methodNaame={0}, request={1})", methodName, request);
            // TODO
            return new DuplexStreamingCall();
        }

        /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
        protected override Client NewInstance(ClientBaseConfiguration configuration)
        {
            return new Client(configuration, serviceName);
        }
    }  // class Client
}  // namespace GrpcToLua
