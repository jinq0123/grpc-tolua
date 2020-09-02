# Todo

* Dead when server is off.
	```
	LuaException: event:172: System/coroutine:127: One or more errors occurred.
	stack traceback:
		[C]: in function '__index'
		grpctolua/Call:38: in function 'await_call'
		RouteGuide:33: in function <RouteGuide:31>
	System.Threading.Tasks.Task`1:get_Result()
	System_Threading_Tasks_Task_bytesWrap:get_Result(IntPtr) (at Assets/Source/Generate/System_Threading_Tasks_Task_bytesWrap.cs:362)
	LuaInterface.LuaDLL:tolua_lateupdate(IntPtr)
	LuaInterface.LuaStatePtr:LuaLateUpdate() (at Assets/ToLua/Core/LuaStatePtr.cs:623)
	LuaLooper:LateUpdate() (at Assets/ToLua/Misc/LuaLooper.cs:113)
	```
* Is lua.Collect() necessary?
* Allow to add proto descriptor file in any order
	+ Provide a method to verify all dependencies are added
* include_imports
	+ protoc --descriptor_set_out=out.pb --include_imports a.proto b.proto
* Client.call() returns immediately a Call object
	+ no need to run in coroutine
	+ in most cases, ignoring the response, no need to wait for the response
* Client.await_call(), Client.async_call()
	* await_call() 仅用于 unary, 阻塞并返回 response
	* async_call() 立即返回 AsyncCall 对象
		- 可能分成4个不同类会好点
* test to response an empty message, return nil or byte[]?
* Secure channel
* Extract c# layer: dynamic grpc
* Stop coroutine on quit