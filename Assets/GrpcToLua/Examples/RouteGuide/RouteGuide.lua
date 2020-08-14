﻿local grpctolua = require('grpctolua')

local channel = grpctolua.new_channel('localhost:50052')
local client = grpctolua.new_client(channel, 'routeguide.RouteGuide')

local pb_file = UnityEngine.Application.dataPath .. '/GrpcToLua/Examples/RouteGuide/protos/route_guide.pb'
print('load proto file descriptor set from: ' .. pb_file)
grpctolua.load_descriptor_set_from_file(pb_file)

function TestGetFeature()
    print('TestGetFeature')
    coroutine.start(CoGetFeature)
end

function TestListFeatures()
    print('TestListFeatures')
    coroutine.start(CoListFeatures)
end

function TestRecordRoute()
    print('TestRecordRoute')
    coroutine.start(CoRecordRoute)
end

function TestRouteChat()
    print('TestRouteChat')
    coroutine.start(CoRouteChat)
end

function CoGetFeature()
    print('CoGetFeature')
    feature = client:call('GetFeature', GetPoint(409146138, -746188906))
    print('feature: '..DumpTable(feature))
end

function CoListFeatures()
    print('CoListFeatures')
end

function CoRecordRoute()
    print('CoRecordRoute')
end

function CoRouteChat()
    print('CoRouteChat')
end

function GetPoint(latitude, longitude)
    return { Latitude = latitude, Longitude = longitude }
end

function DumpTable(t)
    local s = '{'
    for pos, val in pairs(t) do
        s = s .. string.format('[%q] => %q, ', pos, val)
    end
    return s .. '}'
end
