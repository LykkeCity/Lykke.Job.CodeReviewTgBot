using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Job.CodeReviewTgBot.Core.Domain.PullRequests
{
    public interface IActivePullRequestsRepository
    {
        Task<IActivePullRequest> GetAsync(string activePullRequestId);

        Task<IEnumerable<IActivePullRequest>> GetAllAsync(Func<IActivePullRequest, bool> filter = null);

        Task<bool> SaveAsync(IActivePullRequest activePullRequest);

        Task<bool> SaveRangeAsync(IEnumerable<IActivePullRequest> activePullRequests);

        Task RemoveAsync(string activePullRequestId);
    }
}
