using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Job.CodeReviewTgBot.Core.Domain.PullRequests
{
    public interface IActivePullRequest : IEntity
    {
        long? ChatId { get; set; }
        string PullRequestName { get; set; }
        string PullRequestUrl { get; set; }
    }
}
