using AzureStorage;
using Lykke.Job.CodeReviewTgBot.Core.Domain.PullRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Job.CodeReviewTgBot.AzureRepositories.PullRequests
{
    public class ActivePullRequestRepository : IActivePullRequestsRepository
    {
        private readonly INoSQLTableStorage<ActivePullRequest> _tableStorage;

        public ActivePullRequestRepository(INoSQLTableStorage<ActivePullRequest> tableStorage)
        {
            _tableStorage = tableStorage;
        }

        public async Task<IActivePullRequest> GetAsync(string activePullRequestId)
        {
            var pk = ActivePullRequest.GeneratePartitionKey();
            var rk = ActivePullRequest.GenerateRowKey(activePullRequestId);

            return await _tableStorage.GetDataAsync(pk, rk);
        }

        public async Task<IEnumerable<IActivePullRequest>> GetAllAsync(Func<IActivePullRequest, bool> filter = null)
        {
            var pk = ActivePullRequest.GeneratePartitionKey();
            var list = await _tableStorage.GetDataAsync(pk, filter: filter);
            return list as IEnumerable<IActivePullRequest>;
        }

        public async Task<bool> SaveAsync(IActivePullRequest entity)
        {
            try
            {
                if (!(entity is ActivePullRequest apr))
                {
                    apr = (ActivePullRequest)await GetAsync(entity.RowKey) ?? new ActivePullRequest();

                    apr.ETag = entity.ETag;
                }

                apr.PartitionKey = ActivePullRequest.GeneratePartitionKey();
                apr.RowKey = entity.RowKey;
                await _tableStorage.InsertOrMergeAsync(apr);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> SaveRangeAsync(IEnumerable<IActivePullRequest> entities)
        {
            try
            {
                foreach (var item in entities)
                {
                    var entity = (ActivePullRequest)item;
                    if (entity.PartitionKey == null)
                    {
                        entity.PartitionKey = ActivePullRequest.GeneratePartitionKey();
                    }
                    await SaveAsync(entity);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task RemoveAsync(string activePullRequestId)
        {
            var pk = ActivePullRequest.GeneratePartitionKey();
            await _tableStorage.DeleteAsync(pk, activePullRequestId);
        }
    }
}
