using LuaInterface;
using System;
using System.Collections.Generic;
using grpc = global::Grpc.Core;
using pb = Google.Protobuf;
using gpr = global::Google.Protobuf.Reflection;

namespace GrpcToLua
{
    using Marshaller = grpc::Marshaller<LuaTable>;

    public class MarshallerDictionary
    {
        static Dictionary<string, Marshaller> dict = new Dictionary<string, Marshaller>();

        static public Marshaller GetMarshaller(string typeFullName)
        {
            Marshaller result;
            if (dict.TryGetValue(typeFullName, out result))
            {
                return result;
            }

            var newMarshaller = grpc::Marshallers.Create(
                (arg) => Serialize(typeFullName, arg),
                (buf) => Deserialize(typeFullName, buf));
            dict[typeFullName] = newMarshaller;
            return newMarshaller;
        }

        static private byte[] Serialize(string requestTypeFullName, LuaTable request)
        {
            UnityEngine.Debug.LogFormat("Sericalize: {0}", requestTypeFullName);
            gpr.MessageDescriptor msgDesc = DescriptorPool.FindMessage(requestTypeFullName);
            if (msgDesc == null) {
                throw new Exception("Can not serialize request type: " + requestTypeFullName);
            }
            var msg = new LuaMessage(msgDesc, request);
            return msg.ToByteArray();
        }

        static private LuaTable Deserialize(string responseTypeFullName, byte[] responseBuf)
        {
            UnityEngine.Debug.LogFormat("Desericalize: {0}", responseTypeFullName);
            gpr.MessageDescriptor msgDesc = DescriptorPool.FindMessage(responseTypeFullName);
            if (msgDesc == null)
            {
                throw new Exception("Can not deserialize response type: " + responseTypeFullName);
            }
            var ret = LuaState.Get(IntPtr.Zero).NewTable();
            var msg = new LuaMessage(msgDesc, ret);
            msg.MergeFrom(responseBuf);
            return ret;
        }
    }
}