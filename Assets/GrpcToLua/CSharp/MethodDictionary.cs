using LuaInterface;
using System.Collections.Generic;
using grpc = global::Grpc.Core;
using gpr = global::Google.Protobuf.Reflection;
using System;

namespace GrpcToLua
{
    using Method = grpc::Method<LuaTable, LuaTable>;
    using MethodDescriptor = gpr::MethodDescriptor;

    public class MethodDictionary
    {
        readonly string serviceName;
        readonly grpc::MethodType methodType;
        Dictionary<string, Method> dict = new Dictionary<string, Method>();

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

            // TODO: GetServiceMethodRequestAndResponseTypeName(serviceName, methodName)
            string requestTypeName = GetRequestTypeName(methodName);
            string responseTypeName = GetResponseTypeName(methodName);
            var requestMarshaller = MarshallerDictionary.GetMarshaller(requestTypeName);
            var responseMarshaller = MarshallerDictionary.GetMarshaller(responseTypeName);
            var newMethod = new Method(methodType, serviceName, methodName, requestMarshaller, responseMarshaller);
            dict[methodName] = newMethod;
            return newMethod;
        }

        public string GetRequestTypeName(string methodName)
        {
            var methodDesc = GetMethodDescriptor(methodName);
            return methodDesc.InputType.Name;
        }
        public string GetResponseTypeName(string methodName)
        {
            var methodDesc = GetMethodDescriptor(methodName);
            return methodDesc.OutputType.Name;
        }

        private MethodDescriptor GetMethodDescriptor(string methodName)
        {
            string fullName = GetMethodFullName(methodName);
            var methodDesc = DescriptorPool.FindMethod(fullName);
            if (methodDesc != null)
                return methodDesc;
            throw new Exception("No method: " + fullName);
        }

        private string GetMethodFullName(string methodName)
        {
            return serviceName + "." + methodName;
        }
    }  // class MethodDictionary
}  // namespace GrpcToLua
