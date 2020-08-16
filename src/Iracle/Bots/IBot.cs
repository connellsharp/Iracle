using System;
using System.Threading.Tasks;

namespace Iracle
{
    public interface IBot
    {
        Task HandleAsync(BotCommand command);

        event Action<BotEvent> EventHappened;
    }
}