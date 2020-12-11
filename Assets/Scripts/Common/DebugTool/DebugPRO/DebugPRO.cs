using UnityEngine;
using System;
using System.Runtime.CompilerServices;
/*
说明：加强版日志工具，用来在游戏屏幕中实时输出Log信息
 */

public class DebugPRO
{
    public enum DebugLevel
    {
        NONE = -1,
        EXCEPTION,
        ERROR,
        WARNING,
        INFO
    }

    public delegate void PrintRedirect(string msg, global::DebugPRO.DebugLevel type);

    public static global::DebugPRO.DebugLevel logLevel = global::DebugPRO.DebugLevel.INFO;

    public static bool isForbiddenLocal = true;

    private static bool isRegist = false;

    [method: CompilerGenerated]
    [CompilerGenerated]
    public static event global::DebugPRO.PrintRedirect RedirectOutPut;

    public static void RegistExceptionHandler()
    {
        if (global::DebugPRO.isRegist)
        {
            return;
        }
        global::DebugPRO.isRegist = true;
        AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(global::DebugPRO.CurrentDomain_UnhandledException);
        Application.logMessageReceived += new Application.LogCallback(global::DebugPRO.Application_logMessageReceived);
    }

    private static void Application_logMessageReceived(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Exception)
        {
            global::DebugPRO.LogException(logString, stackTrace);
        }
    }

    public static void UnRegistExceptionHandler()
    {
        if (!global::DebugPRO.isRegist)
        {
            return;
        }
        global::DebugPRO.isRegist = false;
        AppDomain.CurrentDomain.UnhandledException -= new UnhandledExceptionEventHandler(global::DebugPRO.CurrentDomain_UnhandledException);
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        global::DebugPRO.LogError(e.ExceptionObject.ToString() + "：" + e.ToString());
    }

    private static void LogByType(string msg, global::DebugPRO.DebugLevel level)
    {
        if (level == global::DebugPRO.DebugLevel.NONE)
        {
            return;
        }
        if (level > global::DebugPRO.logLevel)
        {
            return;
        }
        if (global::DebugPRO.RedirectOutPut != null)
        {
            global::DebugPRO.RedirectOutPut(msg, level);
        }
        if (global::DebugPRO.isForbiddenLocal)
        {
            return;
        }
        switch (level)
        {
            case global::DebugPRO.DebugLevel.ERROR:
                UnityEngine.Debug.LogError(msg);
                return;
            case global::DebugPRO.DebugLevel.WARNING:
                UnityEngine.Debug.LogWarning(msg);
                return;
            case global::DebugPRO.DebugLevel.INFO:
                UnityEngine.Debug.Log(msg);
                return;
            default:
                return;
        }
    }

    public static void LogException(string logString, string stackTrace)
    {
        global::DebugPRO.RedirectOutPut(logString, global::DebugPRO.DebugLevel.EXCEPTION);
        global::DebugPRO.RedirectOutPut(stackTrace, global::DebugPRO.DebugLevel.EXCEPTION);
    }

    public static void LogError(object msg)
    {
        global::DebugPRO.LogByType(msg.ToString(), global::DebugPRO.DebugLevel.ERROR);
    }

    public static void LogWarning(object msg)
    {
        global::DebugPRO.LogByType(msg.ToString(), global::DebugPRO.DebugLevel.WARNING);
    }

    public static void Log(object msg)
    {
        global::DebugPRO.LogByType(msg.ToString(), global::DebugPRO.DebugLevel.INFO);
    }
}