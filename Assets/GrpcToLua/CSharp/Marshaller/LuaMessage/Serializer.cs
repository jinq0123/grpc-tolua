using LuaInterface;
using System.Collections;
using System.IO;
using gpr = Google.Protobuf.Reflection;
using pb = Google.Protobuf;

namespace GrpcToLua
{
    // Serialize LuaMessage
    public class Serializer
    {
        private readonly MemoryStream ms = new MemoryStream();
        private readonly pb.CodedOutputStream output;

        public Serializer()
        {
            output = new pb.CodedOutputStream(ms);
        }

        public byte[] Serialize(LuaTable tbl, gpr.MessageDescriptor msgDesc)
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
                WriteField(key.ToString(), item.Value, msgDesc);
            }

            return ms.GetBuffer();
        }

        private void WriteField(string fieldName, object value, gpr.MessageDescriptor msgDesc)
        {
            gpr.FieldDescriptor fieldDesc = msgDesc.FindFieldByName(fieldName);
            if (fieldDesc == null)
            {
                // TODO: Warn if got unknown field. Disable warn if has DISABLE_PB_KNOWN_FIELD.
                return;
            }
            int fieldNumber = fieldDesc.FieldNumber;
            gpr.FieldType fieldType = fieldDesc.FieldType;
            UnityEngine.Debug.LogFormat("field: ({0}){1} repeated:{2} type:{3} value:{4}",
                fieldNumber, fieldDesc.FullName, fieldDesc.IsRepeated, fieldType, value);
            if (fieldDesc.IsRepeated)
            {
                WriteRepeatedField(fieldNumber, fieldType, value as LuaTable);
                return;
            }
            WriteField(fieldNumber, fieldType, value);
        }

        private void WriteRepeatedField(int fieldNumber, gpr.FieldType fieldType, LuaTable values)
        {
            UnityEngine.Debug.LogFormat("WriteRepeatedFiledTo()");
            if (values == null)
            {
                return;
            }
            // TODO
        }

        private void WriteField(int fieldNumber, gpr.FieldType fieldType, object value)
        {
            // TODO
        }

    }  // class Serializer
}  // namespace GrpcToLua
