local M = {}

local Client = require("grpctolua.Client")

function M.load_descriptor_set_from_file(file_path)
    -- TODO
end

function M.load_descriptor_set_from_string(s)
    -- TODO
end

function M.new_channel(target)
    return Grpc.Core.Channel(target, GrpcToLua.InsecureCredentials())
end

-- TODO: channel with credentials

function M.new_client(channel, service_name)
    return Client(channel, service_name)
end

return M