using System;
using System.Threading;
using System.Threading.Tasks;

namespace Iracle
{
    /// <summary>
    /// Encapsulates raw IRC protocol communication and exposes IRC-specific methods and events.
    /// </summary>
    public class IrcClient : IDisposable
    {
        private readonly ILineCommunicator _lineCommunicator;

        public IrcClient(ILineCommunicator lineCommunicator)
        {
            _lineCommunicator = lineCommunicator;
            _lineCommunicator.LineReceived += OnLineReceived;
        }

        public bool IsLoggedIn { get; private set; }

        public async Task LoginAsync(IIrcLoginSettings loginSettings, CancellationToken ct = default)
        {
            await _lineCommunicator.ConnectAsync(ct);

            if (loginSettings.Password != null)
                await SetPasswordAsync(loginSettings.Password);

            if (loginSettings.Nick != null)
                await SetNickAsync(loginSettings.Nick);

            if (loginSettings.User != null)
                await SetUserAsync(loginSettings.User);

            while (IsLoggedIn == false)
                await Task.Delay(50, ct);

            foreach (var channel in loginSettings.Channels)
                await JoinChannelAsync(channel);
        }

        private Task SetPasswordAsync(string password)
        {
            return _lineCommunicator.WriteLineAsync("PASS " + password);
        }

        private Task SetNickAsync(string nick)
        {
            return _lineCommunicator.WriteLineAsync("NICK " + nick);
        }

        private Task SetUserAsync(string user)
        {
            return _lineCommunicator.WriteLineAsync("USER " + user + " 0 * :" + user);
        }

        public Task JoinChannelAsync(string channel)
        {
            return _lineCommunicator.WriteLineAsync("JOIN " + channel);
        }

        private Task PongAsync(string message)
        {
            return _lineCommunicator.WriteLineAsync("PONG " + message);
        }

        public Task SendMessageAsync(string channel, string message)
        {
            return _lineCommunicator.WriteLineAsync("PRIVMSG " + channel + " :" + message);
        }

        public event PingReceivedHandler PingReceived;

        public event LoggedInHandler LoggedIn;

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
                IsLoggedIn = true;
                LoggedIn?.Invoke();
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

    public delegate void LoggedInHandler();
}
