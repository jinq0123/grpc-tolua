# Todo

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