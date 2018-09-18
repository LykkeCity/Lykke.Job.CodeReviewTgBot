using Autofac;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Job.CodeReviewTgBot.AzureRepositories.PullRequests;
using Lykke.Job.CodeReviewTgBot.Core.Domain.PullRequests;
using Lykke.Job.CodeReviewTgBot.Settings;
using Lykke.SettingsReader;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Job.CodeReviewTgBot.Modules
{
    public class DbModule : Module
    {
        private readonly IReloadingManager<AppSettings> _settings;

        public DbModule(IReloadingManager<AppSettings> settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => 
            new ActivePullRequestRepository(
                AzureTableStorage<ActivePullRequest>.Create( _settings.Nested(s => s.CodeReviewTgBotJob.Db.ConnectionString), 
                "ActivePullRequests", c.Resolve<ILogFactory>())))
                .As<IActivePullRequestsRepository>()
                .SingleInstance();
        }
    }
}
