using System.Threading;
using System.Threading.Tasks;

namespace Iracle
{
    public interface ILineCommunicator
    {
        Task ConnectAsync(CancellationToken ct = default);
        
        void WriteLine(string line);
        
        event LineReceived LineReceived;
    }

    public delegate void LineReceived(string line);
}
