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
    local methodInfo = assert(self.client:GetMethodInfo(method_name))
    local request_data = pb.encode(methodInfo.input_type, request)
    local call = self.client:UnaryCall(method_name, request_data)
    await(call)
    print(string.format("call: %q(%s)", call, type(call)))
    local status = call:GetStatus()
    print(string.format("status: %q(%s)", status, type(status)))
    local resp = call.ResponseAsync
    print(string.format("resp: %q(%s)", resp, type(resp)))
    local result = resp.Result
    print(string.format("result: %q(%s)", result, type(result)))
    local respMsg = pb.decode(methodInfo.output_type, tolua.tolstring(result))
    return respMsg
end

return Client