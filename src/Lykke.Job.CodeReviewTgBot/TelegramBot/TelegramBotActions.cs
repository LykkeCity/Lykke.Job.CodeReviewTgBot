using Lykke.Job.CodeReviewTgBot.Settings.JobSettings;
using Octokit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Job.CodeReviewTgBot.TelegramBot
{
    public class TelegramBotActions
    {
        private static GitHubClient client = new GitHubClient(new ProductHeaderValue(CodeReviewTgBotSettings.BotName));

        private static string _organisation;

        public TelegramBotActions(string organisation, string token)
        {
            _organisation = organisation.ToLower().Replace(' ', '-');
            var tokenAuth = new Credentials(token);
            client.Credentials = tokenAuth;
        }

        public bool HasNewPulls


    }
}
