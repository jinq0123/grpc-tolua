using LuaInterface;
using System.Collections.Generic;
using grpc = global::Grpc.Core;

namespace GrpcToLua
{
    using Method = grpc::Method<LuaTable, LuaTable>;

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
            string requestTypeName = "todo";
            string responseTypeName = "todo";
            var requestMarshaller = MarshallerDictionary.GetMarshaller(requestTypeName);
            var responseMarshaller = MarshallerDictionary.GetMarshaller(responseTypeName);
            var newMethod = new Method(methodType, serviceName, methodName, requestMarshaller, responseMarshaller);
            dict[methodName] = newMethod;
            return newMethod;
        }
    }
}