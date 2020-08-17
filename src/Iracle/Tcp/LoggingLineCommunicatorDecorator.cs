using System;
using System.Threading;
using System.Threading.Tasks;

namespace Iracle
{
    public class LoggingLineCommunicatorDecorator : ILineCommunicator, IDisposable
    {
        private readonly ILineCommunicator _inner;

        public LoggingLineCommunicatorDecorator(ILineCommunicator inner)
        {
            _inner = inner;
            _inner.LineReceived += InnerLineReceived;
        }

        public async Task ConnectAsync(CancellationToken ct = default)
        {
            Console.WriteLine("->> Connecting...");
            await _inner.ConnectAsync();
            Console.WriteLine("<<- Connected.");
        }

        public void WriteLine(string line)
        {
            Console.WriteLine("-> " + line);
            _inner.WriteLine(line);
        }

        public event LineReceived LineReceived;

        private void InnerLineReceived(string line)
        {
            Console.WriteLine("<- " + line);
            LineReceived?.Invoke(line);
        }

        public void Dispose()
        {
            _inner.LineReceived -= InnerLineReceived;
        }
    }
}
