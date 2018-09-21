using Lykke.Job.CodeReviewTgBot.AzureRepositories.PullRequests;
using Lykke.Job.CodeReviewTgBot.Settings.JobSettings;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Job.CodeReviewTgBot.TelegramBot
{
    public class TelegramBotActions
    {
        private readonly GitHubClient client;

        private readonly string _organization;

        public TelegramBotActions(string organization, string token, string botName)
        {
            client = new GitHubClient(new ProductHeaderValue(botName));
            _organization = organization.ToLower().Replace(' ', '-');
            var tokenAuth = new Credentials(token);
            client.Credentials = tokenAuth;
        }

        public async Task<IReadOnlyList<Repository>> GetReposInOrganisation()
        {
            return await client.Repository.GetAllForOrg(_organization);
        }

        public async Task<IReadOnlyList<PullRequest>> GetPullsForRepo(long repoId)
        {
            return await client.PullRequest.GetAllForRepository(repoId);
        }

        public async Task<IReadOnlyList<User>> GetUsersForRepo(long repoId)
        {
            var teams = await client.Repository.GetAllTeams(repoId);
            var contributors = await client.Repository.GetAllContributors(repoId);

            var user = contributors.First();
            user.
            var result = new List<User>();
            foreach (var team in teams)
            {
                var users = await client.Organization.Team.GetAllMembers(team.Id);
                result.AddRange(users);
            }
            return result;
        }

    }
}
