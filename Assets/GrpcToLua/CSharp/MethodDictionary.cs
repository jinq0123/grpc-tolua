using LuaInterface;
using System.Collections.Generic;
using grpc = Grpc.Core;
using gpr = Google.Protobuf.Reflection;
using System;

namespace GrpcToLua
{
    // grpc request and response both are string. Lua serializes and deserializes.
    using Method = grpc::Method<string, string>;
    // using MethodDescriptor = gpr::MethodDescriptor;
    using Marshaller = grpc::Marshaller<string>;

    // MethodDictionary maps from method name to Method instance
    public class MethodDictionary
    {
        readonly string serviceName;
        readonly grpc::MethodType methodType;
        readonly Dictionary<string, Method> dict = new Dictionary<string, Method>();

        static readonly Marshaller marshaller = grpc::Marshallers.Create(
                System.Text.Encoding.ASCII.GetBytes,
                System.Text.Encoding.ASCII.GetString
            );

        public MethodDictionary(string serviceName, grpc::MethodType methodType)
        {
            this.serviceName = serviceName;
            this.methodType = methodType;
        }
        
        public Method GetMethod(string methodName)
        {
            Method result;
            if (dict.TryGetValue(methodName, out result))
            {
                return result;
            }

            var newMethod = new Method(methodType, serviceName, methodName, marshaller, marshaller);
            dict[methodName] = newMethod;
            return newMethod;
        }
    }  // class MethodDictionary
}  // namespace GrpcToLua
