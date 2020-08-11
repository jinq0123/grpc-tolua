-- Class follows: http://lua-users.org/wiki/LuaStyleGuide finance/BankAccount.lua

local Client = {}
Client.__index = Client

local function construct(channel, service_name)
    local clt = {}
    setmetatable(clt, Client)

    -- initialize
    clt.channel = channel
    clt.service_name = service_name
    return clt
end

setmetatable(Client, {__call = construct})

function Client:call(method_name, request)
    print(string.format("Client:call(method_name=%q, request=%q)", method_name, request))
    -- TODO
    return { aaa = 123 }
end

return Client