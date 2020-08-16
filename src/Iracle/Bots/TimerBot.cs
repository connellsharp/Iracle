using System;
using System.Threading;
using System.Threading.Tasks;

namespace Iracle
{
    public class TimerBot : IBot
    {
        private readonly ITimer _botTimer;
        private readonly string _channel;
        private readonly Timer _clockTimer;

        public TimerBot(ITimer timer, TimeSpan timeSpan, string channel)
        {
            _botTimer = timer;
            _channel = channel;
            _clockTimer = new Timer(ExecuteTrigger, null, TimeSpan.Zero, timeSpan);
        }

        private void ExecuteTrigger(object state)
        {
            _botTimer.TriggerAsync()
                .ContinueWith(continuation => EventHappened.Invoke(new BotEvent
                {
                    Channel = _channel,
                    Message = continuation.Result
                }));
        }

        public event Action<BotEvent> EventHappened;

        public Task HandleAsync(BotCommand command)
        {
            return Task.CompletedTask;
        }
    }
}