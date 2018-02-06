namespace LelShotter
{
    public static class DataModels
    {
        public struct StatusMessage
        {
            public Level Level;
            public string Message;

            public StatusMessage(Level level, string message)
            {
                Level = level;
                Message = message;
            }
        }

        public enum Level
        {
            Info,
            Success,
            Debug,
            Error
        }

        public enum ScreenshotMode
        {
            FullScreen,
            Selection
        }
    }
}
