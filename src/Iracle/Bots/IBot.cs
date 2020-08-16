using System;
using System.Threading.Tasks;

namespace Iracle
{
    public interface IBot
    {
        Task<string> InvokeCommandAsync(BotCommandContext context, string command);

        event Action<BotEvent> EventHappened;
    }
}