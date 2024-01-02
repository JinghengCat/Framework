using System;
using System.Text;
using UnityEngine;

public enum ELogType
{
    Log,
    Info,
    Warning,
    Error
}

public static class Logger
{
    private static StringBuilder _stringBuilder = new StringBuilder();

    private const string EMPTY_SPACE = " ";
    
    private const string LOG_AFFIX = "LOG";
    private const string INFO_AFFIX = "INFO";
    private const string WARN_AFFIX = "WARN";
    private const string ERROR_AFFIX = "ERROR";

    private const string LOG_FORMAT = "{0} ({1}) [{2}] ";

    public static bool enableInfoLog = true;

    public static void OnInitLogger(bool info)
    {
        enableInfoLog = info;
    }

    public static void Log(string content, params object[] args)
    {
        LogImpl(ELogType.Log, content, args);
    }

    public static void Info(string content, params object[] args)
    {
        if (enableInfoLog)
        {
            LogImpl(ELogType.Info, content, args);
        }
    }

    public static void Warn(string content, params object[] args)
    {
        LogImpl(ELogType.Warning, content, args);
    }
    
    public static void Error(string content, params object[] args)
    {
        LogImpl(ELogType.Error, content, args);
    }
    
    
    
    private static string FormatLogContent(ELogType logType, string content, params object[] args)
    {
        string affix = null;
        switch (logType)
        {
            case ELogType.Info:
                affix = INFO_AFFIX;
            {
                break;
            }
            case ELogType.Warning:
            {
                affix = WARN_AFFIX;
                break;
            }
            case ELogType.Error:
            {
                affix = ERROR_AFFIX;
                break;
            }
            case ELogType.Log:
            default:
            {
                affix = LOG_AFFIX;
                break;
            }
        }
        
        _stringBuilder.Clear();
        _stringBuilder.AppendFormat(LOG_FORMAT, DateTime.Now.ToString(), Time.frameCount, affix);
        _stringBuilder.AppendFormat(content, args);
        
        return _stringBuilder.ToString();
    }
    
    
    private static void LogImpl(ELogType logType, string content, params object[] args)
    {
        string str = FormatLogContent(logType, content, args);

        switch (logType)
        {
            case ELogType.Warning:
            {
                Debug.LogWarning(str);
                break;
            }
            case ELogType.Error:
            {
                Debug.LogError(str);
                break;
            }
            default:
            {
                Debug.Log(str);
                break;
            }
        }
    }
}
