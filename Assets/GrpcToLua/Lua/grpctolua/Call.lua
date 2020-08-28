-- Class follows: http://lua-users.org/wiki/LuaStyleGuide finance/BankAccount.lua
-- Call class wraps c# grpc call object and handles coroutine wait.

local Call = {}
Call.__index = Call

local await = require("grpctolua.await")

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

-- await_response waits for the response and returns it.
-- It must be called in coroutine.
-- TODO: return error
function Call:await_response()
    local call = self.csharp_call
    await(call)
    print(string.format("call: %q(%s)", call, type(call)))
    local status = call:GetStatus()
    print(string.format("status: %q(%s)", status, type(status)))
    local resp = call.ResponseAsync
    print(string.format("resp: %q(%s)", resp, type(resp)))
    local result = resp.Result
    print(string.format("result: %q(%s)", result, type(result)))
    local output_type = self.method_info.output_type
    local resp_msg = pb.decode(output_type, tolua.tolstring(result))
    return resp_msg
end

-- wait_for_each_response waits for the responses and calls the functor for each response.
-- It must be called in coroutin.
-- func is a function whick take the respones message as the parameter:
function Call:wait_for_each_response(func)
    local call = self.csharp_call
    local stream = call.ResponseStream
    print(string.format("stream: %q(%s)", stream, type(stream)))
    stream.MoveNext()
end

-- TODO

return Call
