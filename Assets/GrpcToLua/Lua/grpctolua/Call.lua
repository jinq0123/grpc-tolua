-- Class follows: http://lua-users.org/wiki/LuaStyleGuide finance/BankAccount.lua
-- Call class wraps c# grpc call object and handles coroutine wait.

local Call = {}
Call.__index = Call

local function construct(self, csharp_call, method_info)
    assert(type(csharp_call) == "userdata")
    assert(type(method_info) == "table")
    
    print(string.format("construct(self=%q(%s), csharp_call=%q(%s), method_info=%q(%s))",
        self, type(self), csharp_call, type(csharp_call), method_info, type(method_info)))

    local c = {}
    setmetatable(c, Call)

    -- initialize
    c.csharp_call = csharp_call
    c.method_info = method_info
    return c
end

setmetatable(Call, {__call = construct})

-- TODO
