using System.Threading.Tasks;

namespace Iracle
{
    public interface IResponder
    {
        Task<string> HandleAsync(BotCommand command);
    }
}