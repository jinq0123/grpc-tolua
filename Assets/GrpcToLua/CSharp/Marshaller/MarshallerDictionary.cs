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
            UnityEngine.Debug.LogFormat("msgDesc: {0}, ClrType: {1}", msgDesc, msgDesc.ClrType);
            var msg = (pb.IMessage)Activator.CreateInstance(msgDesc.ClrType);

            IList<gpr.FieldDescriptor> fields = msgDesc.Fields.InFieldNumberOrder();
            for (int i = 0; i < fields.Count; i++)
            {
                gpr.FieldDescriptor fld = fields[i];
                fld.Accessor.SetValue(msg, request[fld.Name]);
            }
            return global::Google.Protobuf.MessageExtensions.ToByteArray(msg);
        }

        static private LuaTable Deserialize(string responseTypeFullName, byte[] responseBuf)
        {
            UnityEngine.Debug.LogFormat("Desericalize: {0}", responseTypeFullName);
            // TODO
            // IMessage message = (IMessage)Activator.CreateInstance(type);
            // message.MergeFrom(input);
            var feature = global::Routeguide.Feature.Parser.ParseFrom(responseBuf);
            var ret = LuaState.Get(IntPtr.Zero).NewTable();
            ret["name"] = feature.Name;
            return ret;
        }
    }
}