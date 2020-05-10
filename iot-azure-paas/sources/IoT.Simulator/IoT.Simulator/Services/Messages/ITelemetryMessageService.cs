using System.Threading.Tasks;

namespace IoT.Simulator.Services
{
    public interface ITelemetryMessageService
    {
        Task<string> GetMessageAsync();
    }
}
