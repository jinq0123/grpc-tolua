-- Class follows: http://lua-users.org/wiki/LuaStyleGuide finance/BankAccount.lua
-- Call class wrap c# grpc call object.

local Call = {}
Call.__index = Call

local function construct(self, csharp_call, method_info)
    print(string.format("construct(self=%q(%s), csharp_call=%q(%s), method_info=%q(%s))",
        self, type(self), csharp_call, type(csharp_call), method_info, type(method_info)))
    assert(type(method_info) == "table")
    local c = {}
    setmetatable(c, Call)

    -- initialize
    c.csharp_call = csharp_call
    return c
end

setmetatable(Call, {__call = construct})

-- TODO



