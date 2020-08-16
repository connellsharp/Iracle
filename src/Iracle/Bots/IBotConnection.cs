namespace Iracle
{
    public interface IBotConnection
    {
        void AddBot(IBot bot);
        void RemoveBot(IBot bot);
    }
}
