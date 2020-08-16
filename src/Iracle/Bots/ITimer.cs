using System.Threading.Tasks;

namespace Iracle
{
    public interface ITimer
    {
        Task<string> TriggerAsync();
    }
}