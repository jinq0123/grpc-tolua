-- http://lua-users.org/wiki/SimpleLuaClasses
local Client = {}
Client.__index = Client

function Client:new(channel, service_name)
    local clt = {}
    setmetatable(clt, Client)

    -- initialize
    clt.channel = channel
    clt.service_name = service_name
    return clt
end

function Client:call(method_name, request)
    print(string.format("Client:call(method_name=%q, request=%q)", method_name, request))
    -- TODO
    return { aaa = 123 }
end

return Client