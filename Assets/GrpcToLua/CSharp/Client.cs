using LuaInterface;
using UnityEngine;
using grpc = global::Grpc.Core;

namespace GrpcToLua
{
    public class Client
    {
        readonly grpc::ChannelBase channel;
        readonly string serviceName;

        MethodDictionary unaryMethods;
        MethodDictionary clientStreamingMethods;
        MethodDictionary serverStreamingMethods;
        MethodDictionary duplexStreamingMethods;

        public Client(grpc::ChannelBase channel, string serviceName)
        {
            this.channel = channel;
            this.serviceName = serviceName;

            unaryMethods = new MethodDictionary(serviceName, grpc.MethodType.Unary);
            clientStreamingMethods = new MethodDictionary(serviceName, grpc.MethodType.ClientStreaming);
            serverStreamingMethods = new MethodDictionary(serviceName, grpc.MethodType.ServerStreaming);
            duplexStreamingMethods = new MethodDictionary(serviceName, grpc.MethodType.DuplexStreaming);
        }

        public UnaryCall UnaryCall(string methodName, LuaTable request)
        {
            Debug.LogFormat("Client.UnaryCall(methodNaame={0}, request={1})", methodName, request);
            // TODO
            var method = unaryMethods.GetMethod(methodName);
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
    }
}
