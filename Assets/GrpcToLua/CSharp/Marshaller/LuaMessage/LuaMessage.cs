using LuaInterface;
using gpr = global::Google.Protobuf.Reflection;

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

        public byte[] ToByteArray()
        {
            return null;  // TODO
        }

        public void MergeFrom(byte[] buf)
        {
            // TODO
        }
    }  // class LuaMessage
}  // namespace GrpcToLua

// IList<gpr.FieldDescriptor> fields = msgDesc.Fields.InFieldNumberOrder();
