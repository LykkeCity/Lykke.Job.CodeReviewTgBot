using Lykke.Job.CodeReviewTgBot.Settings.JobSettings;
using Lykke.Job.CodeReviewTgBot.Settings.SlackNotifications;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.CodeReviewTgBot.Settings
{
    public class AppSettings
    {
        public CodeReviewTgBotSettings CodeReviewTgBotJob { get; set; }

        public SlackNotificationsSettings SlackNotifications { get; set; }

        [Optional]
        public MonitoringServiceClientSettings MonitoringServiceClient { get; set; }
    }
}
