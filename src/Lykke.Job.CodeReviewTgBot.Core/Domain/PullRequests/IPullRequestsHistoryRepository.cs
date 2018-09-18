using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Job.CodeReviewTgBot.Core.Domain.PullRequests
{
    public interface IPullRequestsHistoryRepository
    {
        Task<IPullRequestsHistory> GetAsync(int pullRequestId);

        Task<IEnumerable<IPullRequestsHistory>> GetAllAsync(Func<IPullRequestsHistory, bool> filter = null);

        Task<bool> SaveAsync(IPullRequestsHistory pullRequestHistory);

        Task<bool> SaveRangeAsync(IEnumerable<IPullRequestsHistory> pullRequestsHistory);

        Task RemoveAsync(int pullRequestId);
    }
}
