using System;
using System.IO;

public static class Logger
{
    private static readonly string LogDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
    private static readonly string LogFilePath = Path.Combine(LogDirectory, "errors.txt");
    private static readonly object Lock = new object();
    public static void Log(string message)
    {
        try
        {
            lock (Lock)
            {
                if (!Directory.Exists(LogDirectory))
                {
                    Directory.CreateDirectory(LogDirectory);
                }
                string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}";
                File.AppendAllText(LogFilePath, logEntry);
            }
        }
        catch
        {
        }
    }
}