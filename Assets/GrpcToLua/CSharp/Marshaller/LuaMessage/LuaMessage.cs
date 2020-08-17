using LuaInterface;
using System.Collections;
using System.IO;
using gpr = Google.Protobuf.Reflection;
using pb = Google.Protobuf;

namespace GrpcToLua
{
    // LuaMessage serialize from and deserial to lua table.
    // Because DynamicMessage has not been implemented in protobuf csharp yet, we implement our own.
    public class LuaMessage
    {
        readonly gpr.MessageDescriptor desc;
        readonly LuaTable tbl;

        public LuaMessage(gpr.MessageDescriptor desc, LuaTable tbl)
        {
            this.desc = desc;
            this.tbl = tbl;
        }

        // Serialize
        public byte[] ToByteArray()
        {
            var serializer = new Serializer();
            return serializer.Serialize(tbl, desc);
        }

        // Deserialize
        public void MergeFrom(byte[] data)
        {
            pb.CodedInputStream input = new pb.CodedInputStream(data);
            MergeFrom(input);
        }

        public void MergeFrom(pb.CodedInputStream input)
        {
            // TODO
        }
    }  // class LuaMessage
}  // namespace GrpcToLua

// IList<gpr.FieldDescriptor> fields = msgDesc.Fields.InFieldNumberOrder();
