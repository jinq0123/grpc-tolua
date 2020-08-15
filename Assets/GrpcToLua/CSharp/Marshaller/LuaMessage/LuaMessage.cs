using LuaInterface;
using System.IO;
using gpr = global::Google.Protobuf.Reflection;
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
            MemoryStream ms = new MemoryStream();
            pb.CodedOutputStream output = new pb.CodedOutputStream(ms);
            WriteTo(output);
            return ms.GetBuffer();
        }

        public void WriteTo(pb::CodedOutputStream output)
        {
            var dicTable = tbl.ToDictTable();
            foreach (var item in dicTable)
            {
                UnityEngine.Debug.LogFormat("±éÀútable:{0}--{1}", item.Key, item.Value);
            }
            // TODO
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
