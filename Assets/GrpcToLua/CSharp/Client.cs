using LuaInterface;
using UnityEngine;
using grpc = global::Grpc.Core;

namespace GrpcToLua
{
    public class Client
    {
        readonly grpc::ChannelBase channel;
        readonly string serviceName;

        public Client(grpc::ChannelBase channel, string serviceName)
        {
            this.channel = channel;
            this.serviceName = serviceName;
        }
        
        public UnaryCall UnaryCall(string methodName, LuaTable request)
        {
            Debug.LogFormat("Client.UnaryCall(methodNaame={0}, request={1})", methodName, request);
            // TODO
            return new UnaryCall();
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
