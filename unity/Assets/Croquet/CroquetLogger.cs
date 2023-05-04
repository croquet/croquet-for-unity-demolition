using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CroquetLogHandler : ILogHandler
{
    public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
    {
        Debug.unityLogger.logHandler.LogFormat(logType, context, format, args);
    }

    public void LogException(Exception exception, UnityEngine.Object context)
    {
        Debug.unityLogger.LogException(exception, context);
    }
}

/// <summary>
/// A statically available logger for Croquet.
/// </summary>
public static class CroquetLogger
{
    private static Logger logger;

    static CroquetLogger()
    {
        logger = new Logger(new CroquetLogHandler());
    }

    [System.Diagnostics.Conditional("ENABLE_LOGS")]
    public static void Log(object msg) 
    {
        logger.Log("CROQUET",$"{DateTime.Now}: {msg}");
    }

    [System.Diagnostics.Conditional("ENABLE_LOGS")]
    public static void LogWarning(object msg)
    {
        logger.LogWarning("CROQUET", $"{DateTime.Now}: {msg}");
    }

    [System.Diagnostics.Conditional("ENABLE_LOGS")]
    public static void LogError(object msg)
    {
        logger.LogError("CROQUET", $"{DateTime.Now}: {msg}");
    }
}
