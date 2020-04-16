using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Iracle
{
    public delegate void LineReceived(string line);

    public class TcpLineCommunicator : ILineCommunicator, IDisposable
    {
        private StreamReader _reader;
        private StreamWriter _writer;
        private readonly string _host;
        private readonly int _port;

        public bool Connected { get; private set; }

        public TcpLineCommunicator(string host, int port)
        {
            _host = host;
            _port = port;

            Start();
        }

        private void Start()
        {
            var client = new TcpClient(_host, _port);
            var stream = client.GetStream();
            _reader = new StreamReader(stream);
            _writer = new StreamWriter(stream);

            new Thread(Run).Start();
        }

        public void WriteLine(string line)
        {
            _writer.WriteLine(line);
            _writer.Flush();
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
