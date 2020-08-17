# Design

* protobuf serialization and deserialization are done on the Lua side.
	+ the rpc request and response type is byte[]
	+ uses lua-protobuf