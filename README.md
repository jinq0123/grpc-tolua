# grpc-tolua

unity grpc for lua using tolua

(Work In Progress)

With grpc-tolua, you can use [grpc](https://github.com/grpc/grpc) in Lua in Unity.

It uses [tolua](https://github.com/topameng/tolua) to bind Unity C# and Lua.

* only grpc client, no server

## Usage example

See [`Examples/RouteGuide`](Assets/GrpcToLua/Examples/RouteGuide)

```lua
local grpctolua = require('grpctolua')

local channel = grpctolua.new_channel('localhost:50051')
local client = grpctolua.new_client(channel, 'routeguide.RouteGuide')

grpctolua.load_descriptor_set_from_file('route_guide.pb')

function TestGetFeature()
    coroutine.start(CoGetFeature)
end

function TestListFeatures()
    coroutine.start(CoListFeatures)
end

function TestRecordRoute()
    coroutine.start(CoRecordRoute)
end

function TestRouteChat()
    coroutine.start(CoRouteChat)
end

function CoGetFeature()
    local feature = client:await_call('GetFeature', GetPoint(409146138, -746188906))
    print('feature: '..DumpTable(feature))
end

function CoListFeatures()
    local req = {lo = GetPoint(400000000, -750000000), hi = GetPoint(420000000, -730000000)}
    local call = client:call('ListFeatures', req)
    call:wait_for_each_response(function(rsp)
        print(DumpTable(rsp))
    end)
end

function CoRecordRoute()
    local call = client:call('RecordRoute')
    coroutine.start(function()
        local rsp = call:wait_for_response()
        print('RecordRoute resonse: '..DumpTable(rsp))
    end)
    
    local features = GetFeatures()
    for _, f in ipairs(features) do
        print('call:await_write(location)...')
        call:await_write(f.location)
    end
    call:await_complete()
end

function CoRouteChat()
    local call = client:call('RouteChat')
    coroutine.start(function() CoPrintResponses(call) end)
    
    local notes = GetRouteNotes()
    for _, n in ipairs(notes) do
        call:await_write(n)
    end
    call:await_complete()
end

function CoPrintResponses(call)
    call:wait_for_each_response(function(rsp)
        print('RouteChat response: '..DumpTable(rsp))
    end)
end
```

## How to run example

1. Open `Assets\GrpcToLua\Examples\RouteGuide\RouteGuide.unity`
1. Menu: Lua -> Generate All
1. Run a server on port 50051. For example [go server](https://github.com/grpc/grpc-go/tree/master/examples/route_guide/server)
1. Play

It can only run on Windows 64 because only Plugins\x86_64\tolua.dll is updated to support lua-protobuf.
On other platforms it will be error with: attempt to call field 'load' (a nil value).
Please [Integrate starwing/lua-protobuf](http://changxianjie.gitee.io/unitypartner/2019/10/01/tolua%E4%B8%AD%E4%BD%BF%E7%94%A8protobuf3%E2%80%94%E9%9B%86%E6%88%90lua-protobuf/)
  for other platforms.

## How to update

1. Update to the latest tolua
	* Copy Assets, Unity5.x, Luajit64, Luajit from tolua
	* Copy `coroutine.wait_until(conditionFunc, co)` from https://github.com/woshihuo12/tolua
		+ See commit 441d9f
1. [Integrate starwing/lua-protobuf](http://changxianjie.gitee.io/unitypartner/2019/10/01/tolua%E4%B8%AD%E4%BD%BF%E7%94%A8protobuf3%E2%80%94%E9%9B%86%E6%88%90lua-protobuf/)
	* Plugins\x86_64\tolua.dll is lua-protobuf ready
1. Update grpc_unity_package
	* https://github.com/grpc/grpc/tree/master/src/csharp/experimental#unity
1. Add in `Assets\Editor\Custom\CustomSetting.cs customTypeList`
	```
	public static BindType[] customTypeList =
	{
		...
        // GrpcToLua
        _GT(typeof(Grpc.Core.Channel)),
        _GT(typeof(Grpc.Core.ChannelCredentials)),
        _GT(typeof(Grpc.Core.Status)),
        _GT(typeof(GrpcToLua.AsyncUnaryCall)),
        _GT(typeof(GrpcToLua.AsyncServerStreamingCall)),
        _GT(typeof(GrpcToLua.AsyncClientStreamingCall)),
        _GT(typeof(GrpcToLua.AsyncDuplexStreamingCall)),
        _GT(typeof(GrpcToLua.Client)),
        _GT(typeof(GrpcToLua.DescriptorSetLoader)),
        _GT(typeof(GrpcToLua.InsecureCredentials)),
        _GT(typeof(System.Runtime.CompilerServices.TaskAwaiter)),
        _GT(typeof(System.Runtime.CompilerServices.TaskAwaiter<byte[]>)),
        _GT(typeof(System.Threading.Tasks.Task<byte[]>)),
	}
	```
1. Add in `Assets\ToLua\Editor\ToLuaExport.cs memberFilter`
	```
    public static List<string> memberFilter = new List<string>
    {
        "Task.IsCompletedSuccessfully",
        ...
	}
	```

## Trouble Shooting

### module 'grpctolua' not found
Make sure to add lua search path like this:
```
    lua = new LuaState();
    lua.AddSearchPath(Application.dataPath + "/GrpcToLua/Lua");
```

### module 'pb' not found
Make sure pb is opened:
```
    lua.OpenLibs(LuaDLL.luaopen_pb);
```

### attempt to call field 'load' (a nil value)
Use starwing/lua-protobuf
