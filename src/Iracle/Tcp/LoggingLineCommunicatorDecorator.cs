using System;

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

        private void InnerLineReceived(string line)
        {
            Console.WriteLine("<- " + line);
            LineReceived?.Invoke(line);
        }

        public event LineReceived LineReceived;

        public void WriteLine(string line)
        {
            Console.WriteLine("-> " + line);
            _inner.WriteLine(line);
        }

        public void Dispose()
        {
            _inner.LineReceived -= InnerLineReceived;
        }
    }
}
