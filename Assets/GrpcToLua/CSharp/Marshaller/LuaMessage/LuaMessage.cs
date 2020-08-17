using LuaInterface;
using System.Collections;
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
            LuaDictTable dicTable = tbl.ToDictTable();
            foreach (DictionaryEntry item in dicTable)
            {
                object key = item.Key;
                UnityEngine.Debug.LogFormat("table item: {0}({2})->{1}({3})", key, item.Value, key.GetType(), item.Value.GetType());
                if (!(key is System.String))
                {
                    continue;
                }
                WriteFieldTo(key.ToString(), item.Value, output);
            }
        }

        private void WriteFieldTo(string fieldName, object value, pb::CodedOutputStream output)
        {
            gpr.FieldDescriptor fieldDesc = desc.FindFieldByName(fieldName);
            if (fieldDesc == null)
            {
                // TODO: Warn if got unknown field. Disable warn if has DISABLE_PB_KNOWN_FIELD.
                return;
            }
            UnityEngine.Debug.LogFormat("field: {0} {1} repeated:{2} {3} {4}",
                fieldDesc.FieldNumber, fieldDesc.FullName, fieldDesc.IsRepeated, fieldDesc.FieldType, value);
            if (fieldDesc.IsRepeated)
            {
                // TODO
                return;
            }
            WriteFieldTo(fieldDesc.FieldNumber, fieldDesc.FieldType, value, output);
        }

        private void WriteFieldTo(int fieldNumber, gpr.FieldType fieldType, object value, pb::CodedOutputStream output)
        {
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
