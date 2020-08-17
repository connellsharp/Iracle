using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Iracle
{
    /// <summary>
    /// A bot of bots. Distributes commands to and combines events from all inner bots.
    /// </summary>
    public class AggregateBot : IBot
    {
        private readonly IList<IBot> _bots;

        public AggregateBot()
        {
            _bots = new List<IBot>();
        }

        public Task HandleAsync(BotCommand command)
        {
            var tasks = _bots.Select(b => b.HandleAsync(command));
            return Task.WhenAll(tasks);
        }

        public event Action<BotEvent> EventHappened;

        public void Add(IBot bot)
        {
            _bots.Add(bot);
            bot.EventHappened += OnBotEventHappened;
        }

        public void Remove(IBot bot)
        {
            _bots.Remove(bot);
            bot.EventHappened -= OnBotEventHappened;
        }

        internal void RemoveAll()
        {
            foreach (var bot in _bots.ToArray())
            {
                Remove(bot);
            }
        }

        private void OnBotEventHappened(BotEvent evnt)
        {
            EventHappened?.Invoke(evnt);
        }
    }
}