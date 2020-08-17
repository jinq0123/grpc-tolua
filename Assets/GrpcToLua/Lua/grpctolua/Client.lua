-- Class follows: http://lua-users.org/wiki/LuaStyleGuide finance/BankAccount.lua
-- Client class forwards calls to c# and handles coroutine wait.

local Client = {}
Client.__index = Client

local await = require("grpctolua.await")
local pb = require("pb")  -- lua-protobuf

local function construct(self, channel, service_name)
    print(string.format("construct(self=%q(%s), channel=%q(%s), service_name=%q(%s))", self, type(self), channel, type(channel), service_name, type(service_name)))
    local clt = {}
    setmetatable(clt, Client)

    -- initialize
    clt.client = GrpcToLua.Client(channel, service_name)
    return clt
end

setmetatable(Client, {__call = construct})

function Client:call(method_name, request)
    print(string.format("Client:call(method_name=%q, request=%q)", method_name, request))
    local info = assert(self.client:GetMethodInfo(method_name))
    local request_data = pb.encode(info.input_type, request)
    
    local is_client_streaming = info.is_client_streaming
    local is_server_streaming = info.is_server_streaming
    if is_client_streaming and is_server_streaming then
        return self:duplex_streaming_call(method_name, request_data, info.output_type)
    elseif is_client_streaming then
        return self:client_streaming_call(method_name, request_data, info.output_type)
    elseif info.is_server_streaming then
        return self:server_streaming_call(method_name, request_data, info.output_type)
    end
    self:unary_call(method_name, request_data, info.output_type)
end  -- call()

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
end

return Client