using LuaInterface;
using System;
using System.Collections.Generic;
using UnityEngine;
using grpc = global::Grpc.Core;

namespace GrpcToLua
{
    using Marshaller = grpc::Marshaller<LuaTable>;

    public class MarshallerDictionary
    {
        static Dictionary<string, Marshaller> dict = new Dictionary<string, Marshaller>();

        static public Marshaller GetMarshaller(string typeName)
        {
            Marshaller result;
            if (dict.TryGetValue(typeName, out result))
            {
                return result;
            }

            var newMarshaller = grpc::Marshallers.Create(
                (arg) => Serialize(typeName, arg),
                (buf) => Deserialize(typeName, buf));
            dict[typeName] = newMarshaller;
            return newMarshaller;
        }
        
        static private byte[] Serialize(string requestTypeName, LuaTable request)
        {
            Debug.LogFormat("Sericalize: {0}", requestTypeName);
            // TODO
            var msg = new Routeguide.Point();
            msg.Latitude = 12345;
            return global::Google.Protobuf.MessageExtensions.ToByteArray(msg);
        }
        
        static private LuaTable Deserialize(string responseTypeName, byte[] responseBuf)
        {
            Debug.LogFormat("Desericalize: {0}", responseTypeName);
            // TODO
            var feature = global::Routeguide.Feature.Parser.ParseFrom(responseBuf);
            var ret = LuaState.Get(IntPtr.Zero).NewTable();
            ret["name"] = feature.Name;
            return ret;
        }
    }
}