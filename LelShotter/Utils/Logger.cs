using System;
using System.IO;
using Settings = LelShotter.Properties.Settings;

namespace LelShotter.Utils
{
    public static class Logger
    {
        private const string LogPath = "LelShotter.out.log";
        private const string ErrLogPath = "LelShotter.err.log";

        private static StreamWriter _out;
        private static StreamWriter _err;

        private const string LogFormat = "{0} [{1}]: {2}";


        static Logger()
        {
           OpenStreams();
        }

        private static void OpenStreams()
        {
            if (_out == null && Settings.Default.DebugMode)
            {
                _out = new StreamWriter(LogPath, true) {AutoFlush = true};
            }
            if (_err == null && Settings.Default.VerboseMode)
            {
                _err = new StreamWriter(ErrLogPath, true) {AutoFlush = true};
            }
        }

        public static void Log(Models.Level level, string msg)
        {
            OpenStreams();
            if (level == Models.Level.Info || level == Models.Level.Success)
            {
                if (Settings.Default.DebugMode)
                {
                    LogInfo(msg);
                }
            }
            else if (level == Models.Level.Error)
            {
                if (Settings.Default.VerboseMode)
                {
                    LogError(msg);
                }
            }
            else if (level == Models.Level.Debug)
            {
                if (Settings.Default.DebugMode)
                {
                    LogDebug(msg);
                }
            }
        }

        private static void LogInfo(string msg)
        {
            _out.WriteLine(LogFormat, DateTime.UtcNow.ToString("u"), Models.Level.Info.ToString().ToUpper(), msg);
        }

        private static void LogDebug(string msg)
        {
            _out.WriteLine(LogFormat, DateTime.UtcNow.ToString("u"), Models.Level.Debug.ToString().ToUpper(), msg);
        }

        private static void LogError(string msg)
        {
            var message = string.Format(LogFormat, DateTime.UtcNow.ToString("u"), Models.Level.Error.ToString().ToUpper(), msg);
            _out.WriteLine(message);
            _err.WriteLine(message);
        }

        public static void ToConsoleInfo(string msg)
        {
            var message = string.Format(LogFormat, DateTime.UtcNow.ToString("u"), Models.Level.Info.ToString().ToUpper(), msg);
            Console.WriteLine(message);
            _out.WriteLine(message);
        }

        public static void ToConsoleDebug(string msg)
        {
            var message = string.Format(LogFormat, DateTime.UtcNow.ToString("u"), Models.Level.Debug.ToString().ToUpper(), msg);
            Console.WriteLine(message);
            _out.WriteLine(message);
        }

        public static void ToConsoleError(string msg)
        {
            var message = string.Format(LogFormat, DateTime.UtcNow.ToString("u"), Models.Level.Error.ToString().ToUpper(), msg);
            Console.WriteLine(message);
            _out.WriteLine(message);
        }

        public static void Dispose()
        {
            if (Settings.Default.DebugMode)
            {
                _out.Close();
                _out.Dispose();
            }
            if (Settings.Default.VerboseMode)
            {
                _err.Close();
                _err.Dispose();
            }
        }
    }
}
