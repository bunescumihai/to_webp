using System.Text;

namespace SL.Services;

public class ConversionLogger
{
    private readonly string _logFilePath;
    private static readonly object _lockObject = new object();

    public ConversionLogger()
    {
        var logsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
        if (!Directory.Exists(logsDirectory))
        {
            Directory.CreateDirectory(logsDirectory);
        }
        
        _logFilePath = Path.Combine(logsDirectory, "conversions.log");
    }

    public void LogConversion(int conversionId, int userId, string fileName, int fileSize, string format)
    {
        var logEntry = new StringBuilder();
        logEntry.AppendLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] CONVERSION");
        logEntry.AppendLine($"  Conversion ID: {conversionId}");
        logEntry.AppendLine($"  User ID: {userId}");
        logEntry.AppendLine($"  File Name: {fileName}");
        logEntry.AppendLine($"  File Size: {fileSize} bytes ({FormatFileSize(fileSize)})");
        logEntry.AppendLine($"  Format: {format}");
        logEntry.AppendLine(new string('-', 80));

        WriteToLog(logEntry.ToString());
    }

    public void LogError(int userId, string fileName, string errorMessage)
    {
        var logEntry = new StringBuilder();
        logEntry.AppendLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] ERROR");
        logEntry.AppendLine($"  User ID: {userId}");
        logEntry.AppendLine($"  File Name: {fileName}");
        logEntry.AppendLine($"  Error: {errorMessage}");
        logEntry.AppendLine(new string('-', 80));

        WriteToLog(logEntry.ToString());
    }

    public void LogLimitReached(int userId, int currentCount, int limit)
    {
        var logEntry = new StringBuilder();
        logEntry.AppendLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] LIMIT_REACHED");
        logEntry.AppendLine($"  User ID: {userId}");
        logEntry.AppendLine($"  Current Count: {currentCount}");
        logEntry.AppendLine($"  Limit: {limit}");
        logEntry.AppendLine(new string('-', 80));

        WriteToLog(logEntry.ToString());
    }

    public void LogDeletion(int conversionId, int userId, string fileName)
    {
        var logEntry = new StringBuilder();
        logEntry.AppendLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] DELETION");
        logEntry.AppendLine($"  Conversion ID: {conversionId}");
        logEntry.AppendLine($"  User ID: {userId}");
        logEntry.AppendLine($"  File Name: {fileName}");
        logEntry.AppendLine(new string('-', 80));

        WriteToLog(logEntry.ToString());
    }

    public void LogPlanCreated(int planId, string planName, int limit, int price)
    {
        var logEntry = new StringBuilder();
        logEntry.AppendLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] PLAN_CREATED");
        logEntry.AppendLine($"  Plan ID: {planId}");
        logEntry.AppendLine($"  Plan Name: {planName}");
        logEntry.AppendLine($"  Limit: {limit}");
        logEntry.AppendLine($"  Price: {price}");
        logEntry.AppendLine(new string('-', 80));

        WriteToLog(logEntry.ToString());
    }

    public void LogPlanUpdated(int planId, string oldName, string newName, int oldLimit, int newLimit, int oldPrice, int newPrice)
    {
        var logEntry = new StringBuilder();
        logEntry.AppendLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] PLAN_UPDATED");
        logEntry.AppendLine($"  Plan ID: {planId}");
        logEntry.AppendLine($"  Name: {oldName} → {newName}");
        logEntry.AppendLine($"  Limit: {oldLimit} → {newLimit}");
        logEntry.AppendLine($"  Price: {oldPrice} → {newPrice}");
        logEntry.AppendLine(new string('-', 80));

        WriteToLog(logEntry.ToString());
    }

    public void LogPlanDeleted(int planId, string planName)
    {
        var logEntry = new StringBuilder();
        logEntry.AppendLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] PLAN_DELETED");
        logEntry.AppendLine($"  Plan ID: {planId}");
        logEntry.AppendLine($"  Plan Name: {planName}");
        logEntry.AppendLine(new string('-', 80));

        WriteToLog(logEntry.ToString());
    }

    public void LogUserPlanChanged(int userId, string userEmail, int oldPlanId, string oldPlanName, int newPlanId, string newPlanName)
    {
        var logEntry = new StringBuilder();
        logEntry.AppendLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] USER_PLAN_CHANGED");
        logEntry.AppendLine($"  User ID: {userId}");
        logEntry.AppendLine($"  User Email: {userEmail}");
        logEntry.AppendLine($"  Old Plan: {oldPlanName} (ID: {oldPlanId})");
        logEntry.AppendLine($"  New Plan: {newPlanName} (ID: {newPlanId})");
        logEntry.AppendLine(new string('-', 80));

        WriteToLog(logEntry.ToString());
    }

    public void LogCacheHit(string cacheKey)
    {
        var logEntry = new StringBuilder();
        logEntry.AppendLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] CACHE_HIT");
        logEntry.AppendLine($"  Cache Key: {cacheKey}");
        logEntry.AppendLine(new string('-', 80));

        WriteToLog(logEntry.ToString());
    }

    public void LogCacheMiss(string cacheKey)
    {
        var logEntry = new StringBuilder();
        logEntry.AppendLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] CACHE_MISS");
        logEntry.AppendLine($"  Cache Key: {cacheKey}");
        logEntry.AppendLine($"  Action: Fetching from database");
        logEntry.AppendLine(new string('-', 80));

        WriteToLog(logEntry.ToString());
    }

    public void LogCacheSet(string cacheKey, int itemCount, double durationMinutes)
    {
        var logEntry = new StringBuilder();
        logEntry.AppendLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] CACHE_SET");
        logEntry.AppendLine($"  Cache Key: {cacheKey}");
        logEntry.AppendLine($"  Items Cached: {itemCount}");
        logEntry.AppendLine($"  Duration: {durationMinutes} minutes");
        logEntry.AppendLine(new string('-', 80));

        WriteToLog(logEntry.ToString());
    }

    public void LogCacheCleared(string cacheKey, string reason)
    {
        var logEntry = new StringBuilder();
        logEntry.AppendLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] CACHE_CLEARED");
        logEntry.AppendLine($"  Cache Key: {cacheKey}");
        logEntry.AppendLine($"  Reason: {reason}");
        logEntry.AppendLine(new string('-', 80));

        WriteToLog(logEntry.ToString());
    }

    private void WriteToLog(string message)
    {
        lock (_lockObject)
        {
            try
            {
                File.AppendAllText(_logFilePath, message);
            }
            catch (Exception ex)
            {
                // If logging fails, write to console as fallback
                Console.WriteLine($"Failed to write to log file: {ex.Message}");
                Console.WriteLine(message);
            }
        }
    }

    private string FormatFileSize(int bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }

    public string GetLogFilePath() => _logFilePath;
}
