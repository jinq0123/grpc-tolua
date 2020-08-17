local M = {}

local Client = require("grpctolua.Client")
local pb = require("pb")  -- lua-protobut

-- descriptor set file is generated by:
--   protoc --descriptor_set_out=desc.pb --include_imports a.proto b.proto
function M.load_descriptor_set_from_file(file_path)
    local f = assert(io.open(file_path, "rb"))
    local s = assert(f:read("*all"))
    f:close()
    M.load_descriptor_set_from_string(s)
end

function M.load_descriptor_set_from_string(s)
    GrpcToLua.DescriptorSetLoader.LoadFromLuaByteBuffer(s)
    pb.load(s)
end

function M.new_channel(target)
    return Grpc.Core.Channel(target, GrpcToLua.InsecureCredentials())
end

-- TODO: channel with credentials

function M.new_client(channel, service_name)
    return Client(channel, service_name)
end

return M