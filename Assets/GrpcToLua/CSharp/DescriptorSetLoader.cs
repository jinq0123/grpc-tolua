using System.IO;
using System.Linq;
using gpr = Google.Protobuf.Reflection;
using pb = Google.Protobuf;

namespace GrpcToLua
{
    using FileDescriptorSet = gpr.FileDescriptorSet;
    using FileDescriptor = gpr.FileDescriptor;

    // Load proto files descriptor set.
    // Descriptor set file is generated by:
    //   protoc --descriptor_set_out=desc.pb a.proto b.proto
    //
    // If load multiple descriptor set files, 
    //   any dependencies must come before the descriptor which depends on them.
    // (If A depends on B, and B depends on C, then the descriptors must be presented in the order C, B, A.)
    //
    // If cs code is generated from proto file, you can get Descriptor
    //   from the generated Reflection class and add to pool dirctly:
    //   DescriptorPool.AddFileDescriptor(MyPackage.MyReflection.Descriptor);
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
            foreach (var d in descriptors)
            {
                DescriptorPool.AddFileDescriptor(d);
            }
        }
    }  // class DescriptorSetLoader
}  // namespace GrpcToLua
