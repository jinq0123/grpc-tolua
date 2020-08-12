-- Class follows: http://lua-users.org/wiki/LuaStyleGuide finance/BankAccount.lua

local Client = {}
Client.__index = Client

local function construct(self, channel, service_name)
    print(string.format("construct(self=%q(%s), channel=%q(%s), service_name=%q(%s))", self, type(self), channel, type(channel), service_name, type(service_name)))
    local clt = {}
    setmetatable(clt, Client)

    -- initialize
    clt.client = GrpcToLua.Client.New(channel, service_name)
    return clt
end

setmetatable(Client, {__call = construct})

function Client:call(method_name, request)
    print(string.format("Client:call(method_name=%q, request=%q)", method_name, request))
    -- TODO
    call = self.client:UnaryCall(method_name, request)
    print(string.format("call: %q(%s)", call, type(call)))
    return { aaa = 123 }
end

return Client