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

            var newMarshaller = grpc::Marshallers.Create(Serializer(typeName), Deserializer(typeName));
            dict[typeName] = newMarshaller;
            return newMarshaller;
        }
        
        static private Func<LuaTable, byte[]> Serializer(string typeName)
        {
            // TODO
            return null;
        }
        
        static private Func<byte[], LuaTable> Deserializer(string typeName)
        {
            // TODO
            return null;
        }
    }
}