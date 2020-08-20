# Todo

* Check top warning
```
10:30:21.428-919: Lua stack top is 3
UnityEngine.Debug:LogWarning(Object)
LuaInterface.Debugger:LogWarning(String)
LuaInterface.Debugger:LogWarning(String, Object)
LuaInterface.LuaState:CheckTop() (at Assets/ToLua/Core/LuaState.cs:1157)
LuaLooper:Update() (at Assets/ToLua/Misc/LuaLooper.cs:101)
```

* Is lua.Collect() necessary?
* Allow to add proto descriptor file in any order
	+ Provide a method to verify all dependencies are added
* include_imports
	+ protoc --descriptor_set_out=out.pb --include_imports a.proto b.proto
* Client.call() returns immediately a Call object
	+ no need to run in coroutine
	+ in most cases, ignoring the response, no need to wait for the response
* Call.await_response() will wait in coroutine
* Call.wait_for_each_response() must run in coroutine
* Client.await_call(), Client.async_call()
	* await_call() 仅用于 unary, 阻塞并返回 response
	* async_call() 立即返回 AsyncCall 对象
		- 可能分成4个不同类会好点