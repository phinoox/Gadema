using System;

namespace Gadema.McpServer;

public static class Logger
{
    private static readonly string LogFile = "mcp_server_error.log";
    private static readonly object LockObject = new();

    public static void LogError(string message, Exception? ex = null)
    {
        lock (LockObject)
        {
            try
            {
                using var sw = new System.IO.StreamWriter(LogFile, true);
                sw.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
                if (ex != null)
                {
                    sw.WriteLine($"Exception: {ex.Message}");
                    sw.WriteLine(ex.StackTrace);
                }
            }
            catch
            {
                // If logging fails, we can't do much about it without causing more issues.
                // We might want to write to Console.Error as a fallback.
                Console.Error.WriteLine($"Failed to log error: {message}. Exception: {ex?.Message}");
            }
        }
    }
}
