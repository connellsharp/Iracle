using System;
using System.Threading;
using System.Threading.Tasks;

namespace Iracle
{
    /// <summary>
    /// Encapsulates raw IRC protocol communication and exposes IRC-specific methods and events.
    /// </summary>
    public class IrcCommunicator : IDisposable
    {
        private readonly ILineCommunicator _lineCommunicator;

        public IrcCommunicator(ILineCommunicator lineCommunicator)
        {
            _lineCommunicator = lineCommunicator;
            _lineCommunicator.LineReceived += OnLineReceived;
        }

        public Task ConnectAsync(CancellationToken ct = default)
        {
            return _lineCommunicator.ConnectAsync(ct);
        }

        public void SetPassword(string password)
        {
            _lineCommunicator.WriteLine("PASS " + password);
        }

        public void SetNick(string nick)
        {
            _lineCommunicator.WriteLine("NICK " + nick);
        }

        public void SetUser(string user)
        {
            _lineCommunicator.WriteLine("USER " + user + " 0 * :" + user);
        }

        public void JoinChannel(string channel)
        {
            _lineCommunicator.WriteLine("JOIN " + channel);
        }

        public void Pong(string message)
        {
            _lineCommunicator.WriteLine("PONG " + message);
        }

        public void SendMessage(string channel, string message)
        {
            _lineCommunicator.WriteLine("PRIVMSG " + channel + " :" + message);
        }

        public event PingReceivedHandler PingReceived;

        public event ConnectionReadyHandler ConnectionReady;

        public event MessageReceivedHandler MessageReceived;

        private void OnLineReceived(string line)
        {
            if(line.StartsWith("PING"))
            {
                PingReceived?.Invoke(new PingMessage
                {
                    Message = line.Substring(5)
                });
                return;
            }

            if(!line.StartsWith(":"))
                return;
                
            var sections = line.Split(':');
            var args = sections[1].Split(' ');
            
            if(args[1] == "001")
            {
                ConnectionReady?.Invoke();
                return;
            }
            
            if(args[1] == "PRIVMSG")
            {
                MessageReceived?.Invoke(new PrivateMessage
                {
                    From = new Identity(args[0]),
                    Channel = args[2],
                    Message = sections[2]
                });
                return;
            }

            return;
        }

        public void Dispose()
        {
            _lineCommunicator.LineReceived -= OnLineReceived;
        }
    }

    public delegate void MessageReceivedHandler(PrivateMessage message);

    public delegate void PingReceivedHandler(PingMessage message);

    public delegate void ConnectionReadyHandler();
}
