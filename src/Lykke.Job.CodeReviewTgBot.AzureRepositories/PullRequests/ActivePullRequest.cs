using Lykke.Job.CodeReviewTgBot.Core.Domain.PullRequests;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;

namespace Lykke.Job.CodeReviewTgBot.AzureRepositories.PullRequests
{
    public class ActivePullRequest : TableEntity, IActivePullRequest
    {
        public static string GeneratePartitionKey() => "ActivePR";

        public static string GenerateRowKey(string pullRequestId) => pullRequestId;

        public long? ChatId { get; set ; }

        public string PullRequestName { get; set; }

        public string PullRequestUrl { get; set; }

        public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            if (properties.TryGetValue("ChatId", out var chatId))
            {
                ChatId = chatId.Int64Value;
            }

            if (properties.TryGetValue("PullRequestName", out var pullRequestName))
            {
                PullRequestName = pullRequestName.StringValue;
            }

            if (properties.TryGetValue("PullRequestUrl", out var pullRequestUrl))
            {
                PullRequestUrl = pullRequestUrl.StringValue;
            }

        }

        public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            var dict = new Dictionary<string, EntityProperty>
            {
                { "ChatId", new EntityProperty(ChatId) },
                { "PullRequestName", new EntityProperty(PullRequestName) },
                { "PullRequestUrl", new EntityProperty(PullRequestUrl) },
            };

            return dict;
        }
    }    
}
