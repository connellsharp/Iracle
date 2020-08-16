using System;
using System.Threading.Tasks;

namespace Iracle
{
    public class ResponderBot : IBot
    {
        private readonly IResponder _responder;

        public ResponderBot(IResponder responder)
        {
            _responder = responder;
        }

        public event Action<BotEvent> EventHappened;

        public async Task HandleAsync(BotCommand command)
        {
            var response = await _responder.HandleAsync(command);

            if (response != null)
                EventHappened.Invoke(new BotEvent
                {
                    Channel = command.Channel,
                    Message = response
                });
        }
    }
}