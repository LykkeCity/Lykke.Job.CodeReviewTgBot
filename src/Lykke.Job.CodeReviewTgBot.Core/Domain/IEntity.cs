using System;

namespace Lykke.Job.CodeReviewTgBot.Core.Domain
{
    public interface IEntity
    {
        string RowKey { get; set; }

        string ETag { get; set; }

        DateTimeOffset Timestamp { get; set; }
    }
}
