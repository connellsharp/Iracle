using System.Threading;
using System.Threading.Tasks;

namespace Iracle
{
    public interface ILineCommunicator
    {
        Task ConnectAsync(CancellationToken ct = default);
        
        Task WriteLineAsync(string line, CancellationToken ct = default);
        
        event LineReceived LineReceived;
    }

    public delegate void LineReceived(string line);
}
