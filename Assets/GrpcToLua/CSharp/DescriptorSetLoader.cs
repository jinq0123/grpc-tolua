using Google.Protobuf;
using System.IO;
using System.Linq;
using gpr = global::Google.Protobuf.Reflection;

namespace GrpcToLua
{
    using FileDescriptorSet = gpr.FileDescriptorSet;
    using FileDescriptor = gpr.FileDescriptor;

    // Load proto files descriptor set.
    public static class DescriptorSetLoader
    {
        public static void LoadFromFile(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                LoadFromStream(fs);
            }
        }

        public static void LoadFromString(string s)
        {
            byte[] byteArray = System.Text.Encoding.Default.GetBytes(s);
            LoadFromBuf(byteArray);
        }

        public static void LoadFromStream(Stream stream)
        {
            LoadDescriptorSet(FileDescriptorSet.Parser.ParseFrom(stream));
        }

        public static void LoadFromBuf(byte[] buf)
        {
            LoadDescriptorSet(FileDescriptorSet.Parser.ParseFrom(buf));
        }

        public static void LoadDescriptorSet(FileDescriptorSet descriptorSet)
        {
            var byteStrings = descriptorSet.File.Select(f => f.ToByteString()).ToList();
            var descriptors = FileDescriptor.BuildFromByteStrings(byteStrings);
            descriptors.Select(d => { DescriptorPool.AddSymbol(d); return 0; });
        }
    }  // class DescriptorSetLoader
}  // namespace GrpcToLua
