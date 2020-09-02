﻿local grpctolua = require('grpctolua.grpctolua')

local channel = grpctolua.new_channel('localhost:50051')
local client = grpctolua.new_client(channel, 'routeguide.RouteGuide')

-- Load descriptor set file which is generated by protoc.
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
    local feature = client:await_call('GetFeature', GetPoint(409146138, -746188906))
    print('feature: '..DumpTable(feature))
end

function CoListFeatures()
    print('CoListFeatures')
    local req = {lo = GetPoint(400000000, -750000000), hi = GetPoint(420000000, -730000000)}
    local call = client:call('ListFeatures', req)
    print('ListFeature:')
    call:wait_for_each_response(function(rsp)
        print(DumpTable(rsp))
    end)
end

function CoRecordRoute()
    print('CoRecordRoute')
    local call = client:call('RecordRoute')

    coroutine.start(function()
        local rsp = call:wait_for_response()
        print('RecordRoute resonse: '..DumpTable(rsp))
    end)
    
    local features = GetFeatures()
    for _, f in ipairs(features) do
        print('call:await_write(location)...')
        call:await_write(f.location)
        coroutine.wait(0.1)
    end
    print('call:await_complete()')
    call:await_complete()
end

function CoRouteChat()
    print('CoRouteChat')
    local call = client:call('RouteChat')
    
    coroutine.start(function() CoPrintResponses(call) end)
    
    local notes = GetRouteNotes()
    for _, n in ipairs(notes) do
        print('call:await_write(note)...')
        call:await_write(n)
        coroutine.wait(0.1)
    end
    print('call:await_complete()')
    call:await_complete()
end

function CoPrintResponses(call)
    call:wait_for_each_response(function(rsp)
        print('RouteChat response: '..DumpTable(rsp))
    end)
    print("end of CoPrintResponses()")
end

function GetPoint(latitude, longitude)
    return {
        latitude = latitude,
        longitude = longitude,
        [123] = 456,
        [{1,2,3}] = "abc",
        ints = {1,2,3},
    }
end

function DumpTable(t)
    local s = '{'
    for pos, val in pairs(t) do
        s = s .. string.format('[%q] => %q, ', pos, val)
    end
    return s .. '}'
end

function GetFeatures()
    return {
        {location = GetPoint(407838351, -746143763), name = "Patriots Path, Mendham, NJ 07945, USA"},
        {location = GetPoint(408122808, -743999179), name = "101 New Jersey 10, Whippany, NJ 07981, USA"},
        {location = GetPoint(413628156, -749015468), name = "U.S. 6, Shohola, PA 18458, USA"}
    }
end

function GetRouteNotes()
    return {
        NewRouteNote("First message", 0, 0),
        NewRouteNote("Second message", 0, 1),
        NewRouteNote("Third message", 1, 0),
        NewRouteNote("Fourth message", 0, 0)
    }
end

function NewRouteNote(message, lat, lon)
    return {
        message = message,
        location = GetPoint(lat, lon)
    }
end
