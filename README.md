# grpc-tolua
unity grpc for lua using tolua

(Work In Progress)

With grpc-tolua, you can use [grpc](https://github.com/grpc/grpc) in Lua in Unity.

It uses [tolua](https://github.com/topameng/tolua) to bind Unity C# and Lua.

* only grpc client, no server

## Usage example

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