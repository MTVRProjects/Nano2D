
using UnityEngine;
using System;
using System.IO;
using System.Text;
/// <summary>
/// 游戏Logo管理
/// </summary>
public class GameLog 
{
    /// <summary>
    /// 日志路径
    /// </summary>
    private static string outPath;

    /// <summary>
    /// 是否开启日志打印
    /// </summary>
    private static bool LogOpen = true;
    /// <summary>
    /// 是否打印常规日志
    /// </summary>
    private static bool IsProcess = true;

    //写流
    static StreamWriter writer = null;
    //日志信息
    static String msg = String.Empty;
    static GameLog()
    {
        outPath = System.IO.Directory.GetCurrentDirectory() + "Log/Log.txt";
    }
    /// <summary>
    /// 写入系统信息作为日志头
    /// </summary>
    private static void writeLogHead()
    {
        try
        {
            writer = new StreamWriter(outPath, false, Encoding.UTF8);
            //运行时间
            writer.WriteLine("启动时间：{0}", DateTime.Now.ToString());
            //设备名称，及类型
            writer.WriteLine("设备名称：{0}；设备类型：{1}", SystemInfo.deviceName, SystemInfo.deviceType);
            //操作系统，内存容量
            writer.WriteLine("操作系统：{0}；内存容量：{1}", SystemInfo.operatingSystem, SystemInfo.systemMemorySize);
            //渲染设备及版本，显存信息
            writer.WriteLine("GPU：{0}；显卡驱动版本号：{1}：显存大小：{2}", SystemInfo.graphicsDeviceName, SystemInfo.graphicsDeviceVersion, SystemInfo.graphicsMemorySize);
            //CPU设备类型及内核数，频率
            writer.WriteLine("CPU：{0}；内核数：{1}；频率：{2}", SystemInfo.processorType, SystemInfo.processorCount, SystemInfo.processorFrequency);
            //设备唯一序号
            writer.WriteLine("设备唯一序号：{0}", SystemInfo.deviceUniqueIdentifier);
            //项目资源路径
            writer.WriteLine("项目资源路径：{0}", Application.dataPath);
            //持久资源路径
            writer.WriteLine("持久资源路径：{0}", Application.persistentDataPath);
            //临时缓冲资源路径
            writer.WriteLine("临时缓冲路径：{0}", Application.temporaryCachePath);
            //流资源路径
            writer.WriteLine("流资源路径：{0}\n", Application.streamingAssetsPath);
        }
        catch (Exception er)
        {
            Debug.LogError(string.Format("日志头写入错误:\r\n{0}\r\n{1}", er.StackTrace, er.Message));
        }
        finally
        {
            if (writer != null)
            {
                writer.Close();
                writer = null;
            }
        }
    }
    /// <summary>
    /// 写入日志到文本
    /// </summary>
    internal static void WriteLog(string logstr)
    {
        try
        {
            writer = new StreamWriter(outPath, true, Encoding.UTF8);
            writer.WriteLine(logstr);
        }
        catch (Exception err)
        {
            Debug.LogError(string.Format("入日志错误:\r\n{0}\r\n{1}\r\n{2}", logstr, err.StackTrace, err.Message));
        }
        finally
        {
            if (writer != null)
            {
                writer.Close();
                writer = null;
            }
        }
    }
    /// <summary>
    /// 监控日志
    /// </summary>
    private static void logHandle(string logString, string stackTrace, LogType type)
    {
        msg = String.Empty;
        switch (type)
        {
            case LogType.Log:
                if (IsProcess) msg = string.Format("{0}\n{1}", logString, stackTrace);
                break;
            default:
                msg = string.Format("{0}错误日志:\n{1}\n{2}", type.ToString(), logString, stackTrace);
                break;
        }
        if (msg != String.Empty)
        {
            WriteLog(msg.ToString());
        }
    }
    /// <summary>
    /// 开启记录日志
    /// </summary>
    public static void Open()
    {
        if (LogOpen)
        {
            writeLogHead();
            WriteLog(string.Format("是否开启自定义日志:{0}", IsProcess));
            Application.logMessageReceived += logHandle;
        }
    }
}
