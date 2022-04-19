using System;
using System.IO;
using System.Text;
using UnityEngine;
public enum LogLevel
{
    All = 1,
}

public static class Debugger
{
    private static string frameFile = "./frame_msg_out.data";
    private static string logFile = "./log_info.txt";
    public static void ClearFrame()
    {
        StreamWriter writer = null;
        try
        {
            writer = new StreamWriter(frameFile, false, Encoding.UTF8);
            writer.WriteLine("");
        }
        catch (Exception)
        {
            Debugger.Log("Exception happen in StreamWriter");
        }
        finally
        {
            if (writer != null)
                writer.Dispose();
        }
    }
    public static void LogFrame(string data)
    {
        StreamWriter writer = null;
        try
        {
            writer = new StreamWriter(frameFile, true, Encoding.UTF8);
            writer.WriteLine(data);
        }
        catch (Exception)
        {
            Debugger.Log("Exception happen in StreamWriter");
        }
        finally
        {
            if (writer != null)
                writer.Dispose();
        }
    }
    public static void Log(string data)
    {
        StreamWriter writer = null;
        try
        {
            writer = new StreamWriter(logFile, true, Encoding.UTF8);
            writer.WriteLine(data);
        }
        catch (Exception)
        {
            Debugger.Log("Exception happen in StreamWriter");
        }
        finally
        {
            if (writer != null)
                writer.Dispose();
        }
        Debug.Log(data);
    }
}