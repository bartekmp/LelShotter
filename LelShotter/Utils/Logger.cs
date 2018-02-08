using System;
using System.IO;

namespace LelShotter.Utils
{
    public static class Logger
    {
        private const string LogPath = "LelShotter.out.log";
        private const string ErrLogPath = "LelShotter.err.log";

        private static readonly StreamWriter Out;
        private static readonly StreamWriter Err;

        private const string LogFormat = "{0} [{1}]: {2}";


        static Logger()
        {
            Out = new StreamWriter(LogPath, true);
            Err = new StreamWriter(ErrLogPath, true);

            Out.AutoFlush = true;
            Err.AutoFlush = true;
        }

        public static void Log(Models.Level level, string msg)
        {
            if (level == Models.Level.Info || level == Models.Level.Success)
            {
                LogInfo(msg);
            }
            else if (level == Models.Level.Error)
            {
                LogError(msg);
            }
            else if (level == Models.Level.Debug)
            {
                LogDebug(msg);
            }
        }

        public static void LogInfo(string msg)
        {
            Out.WriteLine(LogFormat, DateTime.UtcNow.ToString("u"), Models.Level.Info.ToString().ToUpper(), msg);
        }

        public static void LogDebug(string msg)
        {
            Out.WriteLine(LogFormat, DateTime.UtcNow.ToString("u"), Models.Level.Debug.ToString().ToUpper(), msg);
        }

        public static void LogError(string msg)
        {
            var message = string.Format(LogFormat, DateTime.UtcNow.ToString("u"), Models.Level.Error.ToString().ToUpper(), msg);
            Out.WriteLine(message);
            Err.WriteLine(message);
        }

        public static void ToConsoleInfo(string msg)
        {
            var message = string.Format(LogFormat, DateTime.UtcNow.ToString("u"), Models.Level.Info.ToString().ToUpper(), msg);
            Console.WriteLine(message);
            Out.WriteLine(message);
        }

        public static void ToConsoleDebug(string msg)
        {
            var message = string.Format(LogFormat, DateTime.UtcNow.ToString("u"), Models.Level.Debug.ToString().ToUpper(), msg);
            Console.WriteLine(message);
            Out.WriteLine(message);
        }

        public static void ToConsoleError(string msg)
        {
            var message = string.Format(LogFormat, DateTime.UtcNow.ToString("u"), Models.Level.Error.ToString().ToUpper(), msg);
            Console.WriteLine(message);
            Out.WriteLine(message);
        }

        public static void Dispose()
        {
            Out.Close();
            Err.Close();
            Out.Dispose();
            Err.Dispose();
        }
    }
}
