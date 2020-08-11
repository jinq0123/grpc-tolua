local M = {}

function M.channel(target)
    return Grpc.Core.Channel.New(target, Grpc.Core.ChannelCredentials.Insecure)
end

-- TODO: channel with credentials

return M