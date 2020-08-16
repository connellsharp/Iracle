using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Iracle
{
    public class IrcConnection : IBotConnection, IDisposable
    {
        private readonly IrcCommunicator _communicator;
        private readonly IrcConnectionSettings _settings;
        private readonly IList<IBot> _bots;

        public IrcConnection(ILineCommunicator lineCommunicator, IrcConnectionSettings settings)
            : this(new IrcCommunicator(lineCommunicator), settings)
        {
        }

        public IrcConnection(IrcCommunicator communicator, IrcConnectionSettings settings)
        {
            _bots = new List<IBot>();

            _communicator = communicator;
            _communicator.ConnectionReady += OnConnectionReady;
            _communicator.MessageReceived += OnMessageReceived;
            _communicator.PingReceived += OnPingReceived;

            _settings = settings;
        }

        public bool Ready { get; private set; }

        public async Task ConnectAsync(CancellationToken ct = default)
        {
            if (_settings.Password != null)
                _communicator.SetPassword(_settings.Password);

            if (_settings.Nick != null)
                _communicator.SetNick(_settings.Nick);

            if (_settings.User != null)
                _communicator.SetUser(_settings.User);

            while (Ready == false)
                await Task.Delay(50, ct);

            foreach (var channel in _settings.Channels)
                _communicator.JoinChannel(channel);
        }

        private void OnConnectionReady()
        {
            Ready = true;
        }

        public void AddBot(IBot bot)
        {
            _bots.Add(bot);
            bot.EventHappened += OnBotEventHappened;
        }

        public void RemoveBot(IBot bot)
        {
            _bots.Remove(bot);
            bot.EventHappened -= OnBotEventHappened;
        }

        private void OnBotEventHappened(BotEvent evnt)
        {
            _communicator.SendMessage(evnt.Channel, evnt.Message);
        }

        private void OnMessageReceived(PrivateMessage message)
        {
            var command = new BotCommand
            {
                Channel = message.Channel,
                From = message.From,
                Message = message.Message
            };

            foreach (var bot in _bots)
            {
                bot.HandleAsync(command); // fire and forget
                // TODO handle async exceptions ?
            }
        }

        private void OnPingReceived(PingMessage message)
        {
            _communicator.Pong(message.Message);
        }

        public void Dispose()
        {
            _communicator.ConnectionReady -= OnConnectionReady;
            _communicator.MessageReceived -= OnMessageReceived;
            _communicator.PingReceived -= OnPingReceived;

            foreach (var bot in _bots.ToArray())
            {
                RemoveBot(bot);
            }
        }
    }
}
