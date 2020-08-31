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

-- wait_for_response waits for the response and returns it.
-- It must be called in coroutine.
-- TODO: return error
function Call:wait_for_response()
    local call = self.csharp_call
    await(call)
    print(string.format("call: %q(%s)", call, type(call)))
    local status = call:GetStatus()
    print(string.format("status: %q(%s)", status, type(status)))
    local resp = call.ResponseAsync
    print(string.format("resp: %q(%s)", resp, type(resp)))
    local result = resp.Result
    print(string.format("result: %q(%s)", result, type(result)))
    return self:_decode_response(result)
end

-- wait_for_each_response waits for the responses and calls the functor for each response.
-- It must be called in coroutine.
-- func is a function whick take the respones message as the parameter:
function Call:wait_for_each_response(func)
    assert(type(func) == "function", "func must be a function")
    local call = self.csharp_call
    local output_type = self.method_info.output_type
    while (true) do
        local task = call:GetNextResponseAsync()
        print(string.format("Call:wait_for_each_response, await(task=%q(%s))", task, type(task)))
        await(task)
        local response_buffer = task.Result
        if response_buffer == nil then
            print("end of response stream")
            return
        end
        -- print(string.format("response_buffer: %q(%s)", response_buffer, type(response_buffer)))
        local response_message = self:_decode_response(response_buffer)
        -- print(string.format("response_message: %q(%s)", response_message, type(response_message)))
        func(response_message)
    end  -- while
end

-- await_write writes to stream and wait for it.
-- It must be called in coroutine.
-- Only one write can be pending at a time.
function Call:await_write(request_message)
    assert(type(request_message) == "table", "request_message must be a table")
    local request_data = self:_encode_request(request_message)
    print("self.csharp_call:WriteAsync(request_data)...")
    local task = self.csharp_call:WriteAsync(request_data)
    print("await(task)...")
    await(task)
    print("await(task) done.")
end  -- await_write()

-- await_complete completes the writing and wait for it.
-- It must be called in coroutine.
function Call:await_complete()
    print("self.csharp_call:CompleteAsync()...")
    local task = self.csharp_call:CompleteAsync()
    print("await(task)...")
    await(task)
    print("await(task) done")
end

-- _decode_response decode response buffer into response message
function Call:_decode_response(response_buffer)
    local output_type = self.method_info.output_type
    -- print("_decode_response: type="..output_type)
    local response_message = pb.decode(output_type, tolua.tolstring(response_buffer))
    return response_message
end

-- _encode_request encode request message into byte buffer
function Call:_encode_request(request_message)
    local input_type = self.method_info.input_type
    return pb.encode(input_type, request_message)
end

-- TODO

return Call
