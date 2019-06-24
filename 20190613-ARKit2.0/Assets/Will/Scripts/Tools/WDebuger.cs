using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Unity 的 Debug 的封装类
/// </summary>
public class WDebuger
{
    /// <summary>
    /// 是否输出打印
    /// </summary>
    public static bool EnableLog = true;
    /// <summary>
    /// 是否显示打印时间
    /// </summary>
    public static bool EnableTime = true;
    /// <summary>
    /// 是否储存到文本
    /// </summary>
    public static bool EnableSave = true;
    /// <summary>
    /// 是否显示堆栈打印信息
    /// </summary>
    public static bool EnableStack = true;
    /// <summary>
    /// 打印文本保存文件夹路径
    /// </summary>
    public static string LogFileDir = null;
    /// <summary>
    /// 打印文本名称
    /// </summary>
    private static string LogFileName = "";
    /// <summary>
    /// 打印前缀
    /// </summary>
    private static string Prefix = "-> ";
    /// <summary>
    /// 打印文本流
    /// </summary>
    private static StreamWriter LogFileWriter = null;
    public static void Log(object message)
    {
        //message = "<color=#00ff00>" + message + " ";
        message = "Log >>>  " + message + " ";
        bool flag = !WDebuger.EnableLog;
        if (!flag)
        {
            string str = WDebuger.GetLogTime() + message;
            UnityEngine.Debug.Log(WDebuger.Prefix + str, null);
            WDebuger.LogToFile("[Log]" + str, true);
        }
    }

    public static void Log(object message, object context)
    {
        message = "Log >>>  " + message + " ";
        bool flag = !WDebuger.EnableLog;
        if (!flag)
        {
            string str = WDebuger.GetLogTime() + message;
            UnityEngine.Debug.Log(WDebuger.Prefix + str, (UnityEngine.Object)context);
            WDebuger.LogToFile("[Log]" + str, true);
        }
    }

    public static void Log(string tag, object message)
    {
        tag = "Log >>>  " + tag + " ";
        message = "Log >>>  " + message + " ";
        bool flag = !WDebuger.EnableLog;
        if (!flag)
        {
            message = WDebuger.GetLogTime(tag, message);
            UnityEngine.Debug.Log(WDebuger.Prefix + message, null);
            WDebuger.LogToFile("[Log]" + message, true);
        }
    }

    public static void Log(string tag, string format, params object[] args)
    {
        tag = "Log >>>  " + tag + " ";
        string message = "Log >>>  " + string.Format(format, args) + " ";
        bool flag = !WDebuger.EnableLog;
        if (!flag)
        {
            string logText = WDebuger.GetLogTime(tag, message);
            UnityEngine.Debug.Log(WDebuger.Prefix + logText, null);
            WDebuger.LogToFile("[Log]" + logText, true);
        }
    }

    public static void LogWarning(object message)
    {
        message = "Log >>>  " + message + " ";
        string str = WDebuger.GetLogTime() + message;
        UnityEngine.Debug.LogWarning(WDebuger.Prefix + str, null);
        WDebuger.LogToFile("[Warning]" + str, true);
    }

    public static void LogWarning(object message, object context)
    {
        message = "Log >>>  " + message + " ";
        string str = WDebuger.GetLogTime() + message;
        UnityEngine.Debug.LogWarning(WDebuger.Prefix + str, (UnityEngine.Object)context);
        WDebuger.LogToFile("[Warning]" + str, true);
    }

    public static void LogWarning(string tag, object message)
    {
        tag = "Log >>>  " + tag + " ";
        message = "Log >>>  " + message + " ";
        message = WDebuger.GetLogTime(tag, message);
        UnityEngine.Debug.LogWarning(WDebuger.Prefix + message, null);
        WDebuger.LogToFile("[Warning]" + message, true);
    }

    public static void LogWarning(string tag, string format, params object[] args)
    {
        tag += "Log >>>  " + tag + " ";
        string message = "Log >>>  " + string.Format(format, args) + " ";
        string logText = WDebuger.GetLogTime(tag, string.Format(format, args));
        UnityEngine.Debug.LogWarning(WDebuger.Prefix + logText, null);
        WDebuger.LogToFile("[Warning]" + logText, true);
    }

    public static void LogError(object message)
    {
        message = "Log >>>  " + message + " ";
        string str = WDebuger.GetLogTime() + message;
        UnityEngine.Debug.LogError(WDebuger.Prefix + str, null);
        WDebuger.LogToFile("[Error]" + str, true);
    }

    public static void LogError(object message, object context)
    {
        message = "Log >>>  " + message + " ";
        string str = WDebuger.GetLogTime() + message;
        UnityEngine.Debug.LogError(WDebuger.Prefix + str, (UnityEngine.Object)context);
        WDebuger.LogToFile("[Error]" + str, true);
    }

    public static void LogError(string tag, object message)
    {
        tag = "Log >>>  " + tag + " ";
        message = "Log >>>  " + message + " ";
        message = WDebuger.GetLogTime(tag, message);
        UnityEngine.Debug.LogError(WDebuger.Prefix + message, null);
        WDebuger.LogToFile("[Error]" + message, true);
    }

    public static void LogError(string tag, string format, params object[] args)
    {
        tag += "Log >>>  " + tag + " ";
        string message = "Log >>>  " + string.Format(format, args) + " ";
        string logText = WDebuger.GetLogTime(tag, string.Format(format, args));
        UnityEngine.Debug.LogError(WDebuger.Prefix + logText, null);
        WDebuger.LogToFile("[Error]" + logText, true);
    }


    /// <summary>
    /// 获取打印时间
    /// </summary>
    /// <param name="tag">触发打印信息对应的类或者NAME字段名称</param>
    /// <param name="message"></param>
    /// <returns></returns>
    private static string GetLogTime(string tag, object message)
    {
        string result = "";
        bool enableTime = WDebuger.EnableTime;
        if (enableTime)
        {
            result = DateTime.Now.ToLocalTime().ToString("HH:mm:ss.fff") + " ";
        }
        return result + tag + "::" + message;
    }

    /// <summary>
    /// 获取打印时间
    /// </summary>
    /// <returns></returns>
    private static string GetLogTime()
    {
        string result = "";
        bool enableTime = WDebuger.EnableTime;
        if (enableTime)
        {
            result = DateTime.Now.ToLocalTime().ToString("HH:mm:ss.fff") + " ";
        }
        return result;
    }
    /// <summary>
    /// 序列化打印信息
    /// </summary>
    /// <param name="message">打印信息</param>
    /// <param name="EnableStack">是否开启堆栈打印</param>
    private static void LogToFile(string message, bool EnableStack = false)
    {
        bool flag = !WDebuger.EnableSave;
        if (!flag)
        {
            bool flag2 = WDebuger.LogFileWriter == null;
            if (flag2)
            {
                WDebuger.LogFileName = DateTime.Now.GetDateTimeFormats('s')[0].ToString();
                WDebuger.LogFileName = WDebuger.LogFileName.Replace("-", "_");
                WDebuger.LogFileName = WDebuger.LogFileName.Replace(":", "_");
                WDebuger.LogFileName = WDebuger.LogFileName.Replace(" ", "");
                WDebuger.LogFileName += ".log";
                bool flag3 = string.IsNullOrEmpty(WDebuger.LogFileDir);
                if (flag3)
                {
                    WDebuger.LogFileDir = WillData.LogFileDir;
                }
                string path = WDebuger.LogFileDir + WDebuger.LogFileName;
                try
                {
                    bool flag4 = !Directory.Exists(WDebuger.LogFileDir);
                    if (flag4)
                    {
                        Directory.CreateDirectory(WDebuger.LogFileDir);
                    }
                    WDebuger.LogFileWriter = File.AppendText(path);
                    WDebuger.LogFileWriter.AutoFlush = true;
                }
                catch (Exception ex2)
                {
                    WDebuger.LogFileWriter = null;
                    UnityEngine.Debug.LogError("LogToCache() " + ex2.Message + ex2.StackTrace, null);
                    return;
                }
            }
            bool flag5 = WDebuger.LogFileWriter != null;
            if (flag5)
            {
                try
                {
                    WDebuger.LogFileWriter.WriteLine(message);
                    bool flag6 = (EnableStack || WDebuger.EnableStack);
                    if (flag6)
                    {
                        WDebuger.LogFileWriter.WriteLine(StackTraceUtility.ExtractStackTrace());
                    }
                }
                catch (Exception)
                {
                }
            }
        }
    }
}