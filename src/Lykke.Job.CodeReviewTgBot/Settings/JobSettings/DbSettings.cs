using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.CodeReviewTgBot.Settings.JobSettings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }
    }
}
