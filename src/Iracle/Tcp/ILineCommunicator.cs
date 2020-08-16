namespace Iracle
{
    public interface ILineCommunicator
    {
        void WriteLine(string line);
        
        event LineReceived LineReceived;
    }

    public delegate void LineReceived(string line);
}
