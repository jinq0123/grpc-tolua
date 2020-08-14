local grpctolua = require('grpctolua')
local channel = grpctolua.new_channel('localhost:50052')
local client = grpctolua.new_client(channel, 'routeguide.RouteGuide')

print("RouteGuide.lua")

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
