using LuaInterface;
using System;
using System.Threading;
using grpc = Grpc.Core;
using gpr = Google.Protobuf.Reflection;

namespace GrpcToLua
{
    using MethodDescriptor = gpr::MethodDescriptor;

    public class Client : grpc::ClientBase<Client>
    {
        readonly string serviceName;
        MethodDictionary unaryMethods;
        MethodDictionary clientStreamingMethods;
        MethodDictionary serverStreamingMethods;
        MethodDictionary duplexStreamingMethods;
        
        static readonly grpc::CallOptions defaultCallOptions = new grpc::CallOptions(null, null, default(CancellationToken));

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

        // Get method info. Used in Lua.
        public LuaTable GetMethodInfo(string methodName)
        {
            // UnityEngine.Debug.LogFormat("GetMethodInfo(methodName='{0}')", methodName);
            var desc = GetMethodDescriptor(methodName);
            if (desc == null) {
                // UnityEngine.Debug.LogFormat("GetMethodDescriptor({0}) returns null", methodName);
                return null;
            }
            var info = LuaState.Get(IntPtr.Zero).NewTable();
            info["input_type"] = desc.InputType.FullName;
            info["output_type"] = desc.OutputType.FullName;
            info["is_client_streaming"] = desc.IsClientStreaming;
            info["is_server_streaming"] = desc.IsServerStreaming;
            return info;
        }

        private MethodDescriptor GetMethodDescriptor(string methodName)
        {
            string fullName = GetMethodFullName(methodName);
            return DescriptorPool.FindMethod(fullName);
        }

        private string GetMethodFullName(string methodName)
        {
            return serviceName + "." + methodName;
        }

        public AsyncUnaryCall AsyncUnaryCall(string methodName, byte[] request)
        {
            UnityEngine.Debug.LogFormat("Client.AsyncUnaryCall(methodName={0}, request={1})", methodName, request);

            var method = unaryMethods.GetMethod(methodName);
            /* TODO: input headers, deadline, cancellationToken
            grpc::Metadata headers = null;
            DateTime? deadline = null;
            CancellationToken cancellationToken = default(CancellationToken);
            var options = new grpc::CallOptions(headers, deadline, cancellationToken);
            */
            var call = CallInvoker.AsyncUnaryCall(method, null, defaultCallOptions, request);
            return new AsyncUnaryCall(call);
        }

        public AsyncServerStreamingCall AsyncServerStreamingCall(string methodName, byte[] request)
        {
            UnityEngine.Debug.LogFormat("Client.AsyncServerStreamingCall(methodNaame={0}, request={1})", methodName, request);
            var method = serverStreamingMethods.GetMethod(methodName);
            var call = CallInvoker.AsyncServerStreamingCall(method, null, defaultCallOptions, request);
            return new AsyncServerStreamingCall(call);
        }
        
        public AsyncClientStreamingCall AsyncClientStreamingCall(string methodName)
        {
            UnityEngine.Debug.LogFormat("Client.AsyncClientStreamingCall(methodNaame={0})", methodName);
            var method = clientStreamingMethods.GetMethod(methodName);
            var call = CallInvoker.AsyncClientStreamingCall(method, null, defaultCallOptions);
            return new AsyncClientStreamingCall(call);
        }
        
        public AsyncDuplexStreamingCall AsyncDuplexStreamingCall(string methodName)
        {
            UnityEngine.Debug.LogFormat("Client.AsyncDuplexStreamingCall(methodNaame={0})", methodName);
            var method = clientStreamingMethods.GetMethod(methodName);
            var call = CallInvoker.AsyncDuplexStreamingCall(method, null, defaultCallOptions);
            return new AsyncDuplexStreamingCall(call);
        }

        /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
        protected override Client NewInstance(ClientBaseConfiguration configuration)
        {
            return new Client(configuration, serviceName);
        }
    }  // class Client
}  // namespace GrpcToLua
