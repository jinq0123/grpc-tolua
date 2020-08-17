using UnityEngine;
using System;
using System.Collections;
using LuaInterface;

// Test RouteGuide grpc using lua
public class RouteGuide : MonoBehaviour 
{
    private LuaState lua = null;
    private LuaLooper looper = null;

    void Awake () 
    {
#if UNITY_5 || UNITY_2017 || UNITY_2018
        Application.logMessageReceived += ShowTips;
#else
        Application.RegisterLogCallback(ShowTips);
#endif
        new LuaResLoader();
        lua = new LuaState();
        lua.OpenLibs(LuaDLL.luaopen_pb);
        lua.AddSearchPath(Application.dataPath + "/GrpcToLua/Lua");
        lua.Start();
        LuaBinder.Bind(lua);
        DelegateFactory.Init();
        looper = gameObject.AddComponent<LuaLooper>();
        looper.luaState = lua;

        string fullPath = Application.dataPath + "/GrpcToLua/Examples/RouteGuide";
        lua.AddSearchPath(fullPath);
        lua.Require("RouteGuide");  // RouteGuide.lua
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

        tips = "";
        CallLuaFunc(funcName);
    }

    void CallLuaFunc(string funcName)
    {
        LuaFunction func = lua.GetFunction(funcName);
        func.Call();
        func.Dispose();
        lua.CheckTop();
    }
}
