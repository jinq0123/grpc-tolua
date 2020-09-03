# How to generate pb file

```
protoc --descriptor_set_out=route_guide.pb --include_imports route_guide.proto
```

Only need file descriptor. No need to generate c# codes.
