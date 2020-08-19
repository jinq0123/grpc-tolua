-- Class follows: http://lua-users.org/wiki/LuaStyleGuide finance/BankAccount.lua
-- Client class forwards calls to c# and returns Call object.

local Client = {}
Client.__index = Client

local await = require("grpctolua.await")
local pb = require("pb")  -- lua-protobuf
local Call = require("grpctolua.Call")

local function construct(self, channel, service_name)
    print(string.format("construct(self=%q(%s), channel=%q(%s), service_name=%q(%s))", self, type(self), channel, type(channel), service_name, type(service_name)))
    local clt = {}
    setmetatable(clt, Client)

    -- initialize
    clt.client = GrpcToLua.Client(channel, service_name)
    return clt
end

setmetatable(Client, {__call = construct})

-- call creates a grpc call object and returns immediately.
-- request must be nil if server streaming
function Client:call(method_name, request)
    assert(type(method_name) == "string")
    
    print(string.format("Client:call(method_name=%q, request=%q)", method_name, request))
    local method_info = assert(self.client:GetMethodInfo(method_name))
    assert(type(method_info) == table)
    
    local clt = self.client
    local is_client_streaming = method_info.is_client_streaming
    if method_info.is_server_streaming then
        assert(request == nil, method_name .. "() is server streaming method, request must be nil")
        if is_client_streaming then
            local csharp_call = clt:AsyncDuplexStreamingCall(method_name)
            return Call(csharp_call, method_info)
        end
        local csharp_call = clt:AsyncServerStreamingCall(method_name)
        return Call(csharp_call, method_info)
    end
    
    assert(type(request) == "table", method_name .. "() request must be a table, but got " .. type(request))
    local request_data = assert(pb.encode(method_info.input_type, request))
    if is_client_streaming then
        local csharp_call = clt:AsyncClientStreamingCall(method_name, request_data)
        return Call(csharp_call, method_info)
    end
    local csharp_call = clt:AsyncUnaryCall(method_name, request_data)
    return Call(csharp_call, method_info)
end  -- call()

-- _call returns c# Call object: 
-- AsyncUnaryCall<T>, AsyncServerStreamingCall<T>, AsyncClientStreamingCall<T> or AsyncDuplexStreamingCall<T>,
--  where T is grpc request type byte[]

--[[
function Client:_call(method_name, request_data, is_server_streaming, is_client_streaming)
    assert(type(request_data) == string)
    local clt = self.client
    if is_client_streaming and is_server_streaming then
        return clt:AsyncDuplexStreamingCall(method_name, request_data)
    elseif is_client_streaming then
        return clt:AsyncClientStreamingCall(method_name, request_data)
    elseif is_server_streaming then
        return clt:AsyncServerStreamingCall(method_name, request_data)
    end
    return clt:AsyncUnaryCall(method_name, request_data)
end  -- call()
]]

--[[
function Client:unary_call(method_name, request_data, output_type)
    local call = self.client:UnaryCall(method_name, request_data)
    await(call)
    print(string.format("call: %q(%s)", call, type(call)))
    local status = call:GetStatus()
    print(string.format("status: %q(%s)", status, type(status)))
    local resp = call.ResponseAsync
    print(string.format("resp: %q(%s)", resp, type(resp)))
    local result = resp.Result
    print(string.format("result: %q(%s)", result, type(result)))
    local respMsg = pb.decode(output_type, tolua.tolstring(result))
    return respMsg
end]]

--[[ assert request is table or nil
local assert_request(request, is_server_streaming, method_name)
    if is_server_streaming then
        assert(request == nil, method_name .. "() is server streaming method, request must be nil")
        return
    end
    request_type = type(request)
    assert(request_type == "table", method_name .. "() request must be a table, but got " .. request_type)
end]]

return Client