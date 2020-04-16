using System;
using System.Threading.Tasks;

namespace Iracle
{
    internal interface IBot
    {
        Task<string> InvokeCommandAsync(BotCommandContext context, string command);

        event Action<BotEvent> EventHappened;
    }
}