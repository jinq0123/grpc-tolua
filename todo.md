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