namespace LelShotter.Models
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
}
