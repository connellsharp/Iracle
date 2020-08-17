using System;
using System.Threading;
using System.Threading.Tasks;

namespace Iracle
{
    /// <summary>
    /// Base class for all types of connections that can have Bots added to them.
    /// </summary>
    public abstract class BotConnection : IDisposable
    {
        private readonly AggregateBot _aggregateBot;

        protected BotConnection()
        {
            _aggregateBot = new AggregateBot();
            _aggregateBot.EventHappened += OnBotEventHappened;
        }

        protected abstract void OnBotEventHappened(BotEvent evnt);

        protected Task SendCommandToBotsAsync(BotCommand command)
        {
            return _aggregateBot.HandleAsync(command);
        }

        public void AddBot(IBot bot)
        {
            _aggregateBot.Add(bot);
        }

        public void RemoveBot(IBot bot)
        {
            _aggregateBot.Remove(bot);
        }

        public abstract Task ConnectAsync(CancellationToken ct = default);

        public virtual void Dispose()
        {
            _aggregateBot.EventHappened -= OnBotEventHappened;
            _aggregateBot.RemoveAll();
        }
    }
}
