-- Class follows: http://lua-users.org/wiki/LuaStyleGuide finance/BankAccount.lua
-- Client class forwards calls to c# and returns Call object.

local Client = {}
Client.__index = Client

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
    assert(type(method_name) == "string", "method_name must be a string")
    
    print(string.format("Client:call(method_name=%q, request=%q)", method_name, request))
    local method_info = assert(self.client:GetMethodInfo(method_name))
    assert(type(method_info) == "table", "method_info must be a table")
    local csharp_call = self:_csharp_call(method_name, request, method_info)
    return Call(csharp_call, method_info)
end  -- call()

-- await_call calls the rpc method and wait for the response.
-- It must be called in a coroutine.
function Client:await_call(method_name, request)
    local call = self:call(method_name, request)
    return call:wait_for_response()
end

-- assert request is table or nil
local function assert_request(request, is_client_streaming, method_name)
    if is_client_streaming then
        assert(request == nil, method_name .. "() is client streaming method, request must be nil")
        return
    end
    local request_type = type(request)
    assert(request_type == "table", method_name .. "() request must be a table, but got " .. request_type)
end

-- _csharp_call returns c# Call object: 
-- AsyncUnaryCall, AsyncServerStreamingCall, AsyncClientStreamingCall or AsyncDuplexStreamingCall,
function Client:_csharp_call(method_name, request, method_info)
    assert_request(request, method_info.is_client_streaming, method_name)
    local clt = self.client
    local is_server_streaming = method_info.is_server_streaming
    if method_info.is_client_streaming then
        if is_server_streaming then
            return clt:AsyncDuplexStreamingCall(method_name)
        end
        return clt:AsyncClientStreamingCall(method_name)
    end
    
    local request_data = assert(pb.encode(method_info.input_type, request))
    if is_server_streaming then
        return clt:AsyncServerStreamingCall(method_name, request_data)
    end
    return clt:AsyncUnaryCall(method_name, request_data)
end  -- _csharp_call()

return Client
