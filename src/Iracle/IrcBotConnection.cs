using System.Threading;
using System.Threading.Tasks;

namespace Iracle
{
    public class IrcBotConnection : BotConnection
    {
        private readonly IrcClient _client;
        private readonly IIrcLoginSettings _loginSettings;

        public IrcBotConnection(IrcSettings settings)
            : this(new LoggingLineCommunicatorDecorator(new TcpLineCommunicator(settings)), settings)
        {
        }

        public IrcBotConnection(ILineCommunicator lineCommunicator, IIrcLoginSettings settings)
        {
            _client = new IrcClient(lineCommunicator);
            _client.MessageReceived += OnMessageReceived;

            _loginSettings = settings;
        }

        public override async Task ConnectAsync(CancellationToken ct = default)
        {
            await _client.LoginAsync(_loginSettings, ct);
        }

        protected async override Task HandleBotEventAsync(BotEvent evnt)
        {
            await _client.SendMessageAsync(evnt.Channel, evnt.Message);
        }

        private void OnMessageReceived(PrivateMessage message)
        {
            var command = new BotCommand
            {
                Channel = message.Channel,
                From = message.From,
                Message = message.Message
            };

            SendCommandToBotsAsync(command).ContinueWith(PublishUnhandledException);
        }

        public override void Dispose()
        {
            _client.MessageReceived -= OnMessageReceived;
            _client.Dispose();

            base.Dispose();
        }
    }
}
