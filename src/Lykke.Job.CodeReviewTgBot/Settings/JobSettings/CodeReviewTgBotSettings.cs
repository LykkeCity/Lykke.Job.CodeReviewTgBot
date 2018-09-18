namespace Lykke.Job.CodeReviewTgBot.Settings.JobSettings
{
    public class CodeReviewTgBotSettings
    {
        public DbSettings Db { get; set; }
        public string BotName { get; set; }
        public string BotToken { get; set; }
        public string GitToken { get; set; }
        public string OrgainzationName { get; set; }
        public string LykkeDevelopersServiceUrl { get; set; }
        public static int TimeoutPeriodSeconds { get; set; }
        public long AllowedGroupId { get; set; }
        public int TotalTimeLimitInMinutes { get; set; }
    }
}
