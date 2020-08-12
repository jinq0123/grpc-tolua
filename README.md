# grpc-tolua

unity grpc for lua using tolua

(Work In Progress)

With grpc-tolua, you can use [grpc](https://github.com/grpc/grpc) in Lua in Unity.

It uses [tolua](https://github.com/topameng/tolua) to bind Unity C# and Lua.

* only grpc client, no server

## Usage example

See [`Examples/RouteGuide`](Assets/GrpcToLua/Examples/RouteGuide)

```
local channel = grpc.Channel.New("localhost:50051")
local stub = grcp.Stub.New(channel, "routeguide.RouteGuide")

-- TODO: stub.Call() should be called in a coroutine

function GetFeature()
	point = {
		Latitude = 409146138,
		Longitude = -746188906
	}
	feature = stub.Call("GetFeature", point)
	print(inspect(feature))
end

function ListFeatures()
	rectangle = {
		Lo = { Latitude = 400000000, Longitude = -750000000 },
		Hi = { Latitude = 420000000, Longitude = -730000000 }
	}
	features = stub.Call("ListFeatures", rectangle)
	for feature in features
		print(inspect(feature))
	end
end

function RecordRoute()
	recorder = stub.Call("RecordRoute")
	for point in points
		recorder.RequestStream.Write(point)
		coroutine.Sleep(0.1)
	end
end

functine RouteChat()
	call = stub.Call("RouteChat")
	responseStream = call.ResponseStream
	coroutine.create(
		function()
			for routeNote in responseStream
				print(inspect(routeNote))
			end  -- for
		end  -- function
	)  -- coroutine.create
	
	for routeNote in requests
		call.RequestStream.Write(routeNote)
	end
end
```

## How to run example

1. Update to the latest tolua
	* Copy Assets, Unity5.x, Luajit64, Luajit from tolua
1. Update grpc_unity_package
	* https://github.com/grpc/grpc/tree/master/src/csharp/experimental#unity
1. Add in `Assets\Editor\Custom\CustomSetting.cs customTypeList`
	```
	public static BindType[] customTypeList =
	{
		...
		_GT(typeof(Grpc.Core.Channel)),
		_GT(typeof(Grpc.Core.ChannelCredentials)),
	}
	```
1. Open `Assets\GrpcToLua\Examples\RouteGuide\RouteGuide.unity`
1. Lua -> Generate All

## Trouble Shooting

### module 'grpctolua' not found
Make sure to add lua search path like this:
```
    lua = new LuaState();
    LuaFileUtils.Instance.AddSearchPath(Application.dataPath + "/GrpcToLua/Lua/?/init.lua");
    // TODO lua.AddSearchPath(Application.dataPath + "/GrpcToLua/Lua");
```