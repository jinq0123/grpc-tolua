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
    return self:decode_response(result)
end

-- wait_for_each_response waits for the responses and calls the functor for each response.
-- It must be called in coroutin.
-- func is a function whick take the respones message as the parameter:
function Call:wait_for_each_response(func)
    assert(type(func) == "function", "func must be a function")
    local call = self.csharp_call
    local output_type = self.method_info.output_type
    while (true) do
        -- tolua must have wait_until(), or merge from https://github.com/woshihuo12/tolua
        coroutine.wait_until(function()
            return call.IsNextResponseReady()
        end)
        
        local rsp = call.GetNextResponse()
        if rsp == nil then
            print("end of response stream")
            return
        end
        print(string.format("rsp: %q(%s)", rsp, type(rsp)))
        response_message = self:decode_response(rsp)
        func(response_messasge)
    end  -- while
end

-- decode_response decode response buffer into response message
function Call:decode_response(response_buffer)
    local output_type = self.method_info.output_type
    local response_message = pb.decode(output_type, tolua.tolstring(response_buffer))
    return response_message
end

-- TODO

return Call
