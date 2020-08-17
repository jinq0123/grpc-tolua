using LuaInterface;
using System.Collections.Generic;
using grpc = Grpc.Core;
using gpr = Google.Protobuf.Reflection;
using System;

namespace GrpcToLua
{
    // grpc TRequest and TResponse are both byte[] (must be reference type).
    // Lua serializes and deserializes.
    using Method = grpc::Method<byte[]/*TRequest*/, byte[]/*TResponse*/>;
    // using MethodDescriptor = gpr::MethodDescriptor;
    using Marshaller = grpc::Marshaller<byte[]>;

    // MethodDictionary maps from method name to Method instance
    public class MethodDictionary
    {
        readonly string serviceName;
        readonly grpc::MethodType methodType;
        readonly Dictionary<string, Method> dict = new Dictionary<string, Method>();

        static readonly Marshaller marshaller =
            grpc::Marshallers.Create((buf)=>buf, (buf)=>buf);

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

            var newMethod = new Method(methodType, serviceName, methodName,
                /*request*/marshaller, /*response*/marshaller);
            dict[methodName] = newMethod;
            return newMethod;
        }
    }  // class MethodDictionary
}  // namespace GrpcToLua
