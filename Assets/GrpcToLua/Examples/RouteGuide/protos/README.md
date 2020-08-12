# Generate from proto

Only file descriptor needed, which can get from cs file or parse from pb file.
It is possible to reload pb file to upgrade proto.

## How to generate cs file

```
protoc --csharp_out=. route_guide.proto
```

## How to generate pb file

```
protoc --descriptor_set_out=route_guide.pb route_guide.proto
```

## How to parse pb file

Search in `protobuf\csharp\src\Google.Protobuf.Test\Reflection`:
```
    descriptorSet = FileDescriptorSet.Parser.ParseFrom(stream);
```