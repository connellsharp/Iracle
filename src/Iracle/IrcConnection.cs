using System.Threading;
using System.Threading.Tasks;

namespace Iracle
{
    public class IrcConnection : BotConnection
    {
        private readonly IrcCommunicator _communicator;
        private readonly IrcConnectionSettings _settings;

        public IrcConnection(ILineCommunicator lineCommunicator, IrcConnectionSettings settings)
            : this(new IrcCommunicator(lineCommunicator), settings)
        {
        }

        public IrcConnection(IrcCommunicator communicator, IrcConnectionSettings settings)
        {
            _communicator = communicator;
            _communicator.ConnectionReady += OnConnectionReady;
            _communicator.MessageReceived += OnMessageReceived;
            _communicator.PingReceived += OnPingReceived;

            _settings = settings;
        }

        public bool Ready { get; private set; }

        public override async Task ConnectAsync(CancellationToken ct = default)
        {
            await _communicator.ConnectAsync(ct);

            if (_settings.Password != null)
                await _communicator.SetPassword(_settings.Password);

            if (_settings.Nick != null)
                await _communicator.SetNick(_settings.Nick);

            if (_settings.User != null)
                await _communicator.SetUser(_settings.User);

            while (Ready == false)
                await Task.Delay(50, ct);

            foreach (var channel in _settings.Channels)
                await _communicator.JoinChannel(channel);
        }

        private void OnConnectionReady()
        {
            Ready = true;
        }

        protected override void OnBotEventHappened(BotEvent evnt)
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

            SendCommandToBotsAsync(command); // fire and forget
            // TODO handle async exceptions ?
        }

        private void OnPingReceived(PingMessage message)
        {
            _communicator.Pong(message.Message); // fire and forget
            // TODO handle async exceptions ?
        }

        public override void Dispose()
        {
            _communicator.ConnectionReady -= OnConnectionReady;
            _communicator.MessageReceived -= OnMessageReceived;
            _communicator.PingReceived -= OnPingReceived;

            base.Dispose();
        }
    }
}
