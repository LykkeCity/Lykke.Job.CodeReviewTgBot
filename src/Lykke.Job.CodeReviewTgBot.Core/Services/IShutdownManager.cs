using System.Threading.Tasks;

namespace Lykke.Job.CodeReviewTgBot.Core.Services
{
    public interface IShutdownManager
    {
        Task StopAsync();
    }
}
