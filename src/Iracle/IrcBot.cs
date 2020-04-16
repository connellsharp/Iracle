using System;
using System.Threading;

namespace Iracle
{
    internal class IrcBot : IDisposable
    {
        private readonly IrcCommunicator _communicator;
        private readonly IBot _bot;
        private readonly IrcBotSettings _settings;

        public IrcBot(IrcCommunicator communicator, IBot bot, IrcBotSettings settings)
        {
            _communicator = communicator;
            _communicator.ConnectionReady += OnConnectionReady;
            _communicator.MessageReceived += OnMessageReceived;
            _communicator.PingReceived += OnPingReceived;

            _bot = bot;
            _bot.EventHappened += OnBotEventHappened;

            _settings = settings;
        }

        public bool Ready { get; private set; }

        public void Connect()
        {
            _communicator.SetNick(_settings.Nick);
            _communicator.SetUser(_settings.User);
            
            while(Ready == false)
                Thread.Sleep(50);

            foreach(var channel in _settings.Channels)
                _communicator.JoinChannel(channel);
        }

        private void OnBotEventHappened(BotEvent evnt)
        {
            _communicator.SendMessage(evnt.Channel, evnt.Message);
        }

        private void OnConnectionReady()
        {
            Ready = true;
        }

        private void OnMessageReceived(PrivateMessage message)
        {
            if(!message.Message.StartsWith("!"))
                return;

            var commandContext = new BotCommandContext 
            {
                Channel = message.Channel,
                From = message.From
            };

            _bot.InvokeCommandAsync(commandContext, message.Message.Substring(1))
                .ContinueWith(task => OnCommandResponse(message, task.Result));
        }

        private void OnCommandResponse(PrivateMessage originalMessage, string response)
        {
            if(response != null)
                _communicator.SendMessage(originalMessage.Channel, response);
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

            _bot.EventHappened -= OnBotEventHappened;
        }
    }
}
