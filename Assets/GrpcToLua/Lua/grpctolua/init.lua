local M = {}

local Client = require("grpctolua.Client")

function M.new_channel(target)
    return Grpc.Core.Channel.New(target, GrpcToLua.InsecureCredentials.New())
end

-- TODO: channel with credentials

function M.new_client(channel, service_name)
    return Client(channel, service_name)
end

return M