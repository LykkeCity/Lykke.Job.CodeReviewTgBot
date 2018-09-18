using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.Job.CodeReviewTgBot.Core.Services;
using Lykke.Job.CodeReviewTgBot.Services;
using Lykke.Job.CodeReviewTgBot.Settings.JobSettings;
using Lykke.Job.CodeReviewTgBot.PeriodicalHandlers;
using Lykke.Job.CodeReviewTgBot.TelegramBot;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;
using Common;

namespace Lykke.Job.CodeReviewTgBot.Modules
{
    public class JobModule : Module
    {
        private readonly CodeReviewTgBotSettings _settings;
        private readonly IReloadingManager<CodeReviewTgBotSettings> _settingsManager;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        public JobModule(CodeReviewTgBotSettings settings, IReloadingManager<CodeReviewTgBotSettings> settingsManager)
        {
            _settings = settings;
            _settingsManager = settingsManager;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            // NOTE: Do not register entire settings in container, pass necessary settings to services which requires them
            // ex:
            // builder.RegisterType<QuotesPublisher>()
            //  .As<IQuotesPublisher>()
            //  .WithParameter(TypedParameter.From(_settings.Rabbit.ConnectionString))

            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>()
                .SingleInstance();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>()
                .SingleInstance();

            builder.RegisterType<TelegramBotService>()
                .As<IStartable>()
                .As<IStopable>()
                .WithParameter(TypedParameter.From(_settings))
                .SingleInstance();

            RegisterPeriodicalHandlers(builder);

            // TODO: Add your dependencies here

            builder.Populate(_services);
        }

        private void RegisterPeriodicalHandlers(ContainerBuilder builder)
        {
            // TODO: You should register each periodical handler in DI container as IStartable singleton and autoactivate it

            builder.RegisterType<CheckPullsHandler>()
                .As<IStartable>()
                .SingleInstance();
        }
    }
}
