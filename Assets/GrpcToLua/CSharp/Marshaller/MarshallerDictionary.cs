using LuaInterface;
using System;
using System.Collections.Generic;
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
            // TODO
            return null;
        }
        
        static private LuaTable Deserialize(string responseTypeName, byte[] responseBuf)
        {
            // TODO
            return null;
        }
    }
}