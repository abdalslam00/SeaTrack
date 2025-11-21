using System;
using System.IO;
using System.Web;

namespace SeaTrack.Utilities
{
    /// <summary>
    /// مساعد تسجيل الأخطاء والأحداث
    /// </summary>
    public class LogHelper
    {
        private static string _logPath = HttpContext.Current != null 
            ? HttpContext.Current.Server.MapPath("~/Logs/") 
            : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

        /// <summary>
        /// تسجيل خطأ
        /// </summary>
        public static void LogError(string method, Exception ex)
        {
            try
            {
                if (!Directory.Exists(_logPath))
                    Directory.CreateDirectory(_logPath);

                string fileName = $"Error_{DateTime.Now:yyyyMMdd}.log";
                string filePath = Path.Combine(_logPath, fileName);

                string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{method}] {ex.Message}\n{ex.StackTrace}\n\n";

                File.AppendAllText(filePath, logMessage);
            }
            catch
            {
                // تجاهل أخطاء التسجيل
            }
        }

        /// <summary>
        /// تسجيل معلومة
        /// </summary>
        public static void LogInfo(string message)
        {
            try
            {
                if (!Directory.Exists(_logPath))
                    Directory.CreateDirectory(_logPath);

                string fileName = $"Info_{DateTime.Now:yyyyMMdd}.log";
                string filePath = Path.Combine(_logPath, fileName);

                string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}\n";

                File.AppendAllText(filePath, logMessage);
            }
            catch
            {
                // تجاهل أخطاء التسجيل
            }
        }

        /// <summary>
        /// تسجيل نشاط المستخدم
        /// </summary>
        public static void LogActivity(string username, string action)
        {
            try
            {
                if (!Directory.Exists(_logPath))
                    Directory.CreateDirectory(_logPath);

                string fileName = $"Activity_{DateTime.Now:yyyyMMdd}.log";
                string filePath = Path.Combine(_logPath, fileName);

                string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{username}] {action}\n";

                File.AppendAllText(filePath, logMessage);
            }
            catch
            {
                // تجاهل أخطاء التسجيل
            }
        }
    }
}
