namespace Iracle
{
    public class BotCommand
    {
        public string Message { get; internal set; }
        public string Channel { get; internal set; }
        public Identity From { get; internal set; }
    }
}