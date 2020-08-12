using UnityEngine;
using System;
using System.Collections;
using LuaInterface;

// Test RouteGuide grpc using lua
public class RouteGuide : MonoBehaviour 
{
    private LuaState lua = null;
    private LuaLooper looper = null;

    string script = @"
        print('script')

        local grpctolua = require('grpctolua')
        local channel = grpctolua.new_channel('localhost:50052')
        local client = grpctolua.new_client(channel, 'routeguide.RouteGuide')

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
            feature = client.call('GetFeature', GetPoint(409146138, -746188906))
            PrintTable('feature', feature)
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

        function PrintTable(name, t)
            print(name..': {')
            for pos, val in pairs(t) do
                print(string.format('    [%q] => %q', pos, val))
            end
            print('}')
        end
    ";  // script

    void Awake () 
    {
#if UNITY_5 || UNITY_2017 || UNITY_2018
        Application.logMessageReceived += ShowTips;
#else
        Application.RegisterLogCallback(ShowTips);
#endif        
        new LuaResLoader();
        lua = new LuaState();
        lua.AddSearchPath(Application.dataPath + "/GrpcToLua/Lua");
        LuaFileUtils.Instance.AddSearchPath(Application.dataPath + "/GrpcToLua/Lua/?/init.lua");
        lua.Start();
        LuaBinder.Bind(lua);
        DelegateFactory.Init();         
        looper = gameObject.AddComponent<LuaLooper>();
        looper.luaState = lua;

        lua.DoString(script, "RouteGuide.cs");
    }

    void OnApplicationQuit()
    {
        looper.Destroy();
        lua.Dispose();
        lua = null;
#if UNITY_5 || UNITY_2017 || UNITY_2018
        Application.logMessageReceived -= ShowTips;
#else
        Application.RegisterLogCallback(null);
#endif
    }

    string tips = null;

    void ShowTips(string msg, string stackTrace, LogType type)
    {
        tips += msg;
        tips += "\r\n";
    }

    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width / 2 - 300, Screen.height / 2 - 200, 600, 400), tips);

        string funcName = null;
        if (GUI.Button(new Rect(50, 50, 120, 45), "GetFeature"))
        {
            funcName = "TestGetFeature";
        }
        else if (GUI.Button(new Rect(50, 100, 120, 45), "ListFeatures"))
        {
            funcName = "TestListFeatures";
        }
        else if (GUI.Button(new Rect(50, 150, 120, 45), "RecordRoute"))
        {
            funcName = "TestRecordRoute";
        }
        else if (GUI.Button(new Rect(50, 200, 120, 45), "RouteChat"))
        {
            funcName = "TestRecordRoute";
        }
        else
        {
            return;
        }
        
        LuaFunction func = lua.GetFunction(funcName);
        func.Call();
        func.Dispose();
    }
}
