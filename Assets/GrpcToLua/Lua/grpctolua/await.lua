-- lua coroutine simulate C# await
-- awaitable is await-expressions
-- https://docs.microsoft.com/zh-cn/dotnet/csharp/language-reference/language-specification/expressions#await-expressions
-- await does not return the result, because in most cases the c# call to get the result is unnecessary.
local function await(awaitable)
    awaiter = awaitable:GetAwaiter()
    -- tolua must have wait_until(), or merge from https://github.com/woshihuo12/tolua
    coroutine.wait_until(function()
        return awaiter.IsCompleted
    end)
end

return await