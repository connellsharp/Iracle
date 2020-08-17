using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Iracle
{
    public class TcpLineCommunicator : ILineCommunicator, IDisposable
    {
        private readonly ITcpConnectionSettings _settings;
        private StreamReader _reader;
        private StreamWriter _writer;

        public bool Connected { get; private set; }

        public TcpLineCommunicator(string host, int port)
        {
            _settings = new TcpConnectionSettings
            {
                Host = host,
                Port = port
            };
        }

        public TcpLineCommunicator(ITcpConnectionSettings settings)
        {
            _settings = settings;
        }

        public async Task ConnectAsync(CancellationToken ct = default)
        {
            var client = new TcpClient();
            await client.ConnectAsync(_settings.Host, _settings.Port);

            var stream = client.GetStream();
            _reader = new StreamReader(stream);
            _writer = new StreamWriter(stream);

            new Thread(Run).Start();
        }

        public async Task WriteLineAsync(string line, CancellationToken ct = default)
        {
            await _writer.WriteLineAsync(line);
            await _writer.FlushAsync();
        }

        public event LineReceived LineReceived;

        private void Run()
        {
            Connected = true;
            while (Connected)
            {
                string line = _reader.ReadLine();
                
                if(line == null)
                {
                    Connected = false;
                    return;
                }

                LineReceived?.Invoke(line); 
            }
        }

        internal void WaitUntilDisconnected()
        {
            while(Connected)
                Thread.Sleep(500);
        }

        public void Dispose()
        {
            Connected = false;
            _reader?.Dispose();
            _writer?.Dispose();
        }
    }
}
