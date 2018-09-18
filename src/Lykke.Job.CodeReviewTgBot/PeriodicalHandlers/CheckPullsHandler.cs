using Common;
using Lykke.Common.Log;
using Lykke.Job.CodeReviewTgBot.Settings.JobSettings;
using System;
using System.Threading.Tasks;

namespace Lykke.Job.CodeReviewTgBot.PeriodicalHandlers
{
    public delegate void CheckPulls();

    public class CheckPullsHandler : TimerPeriod
    {
        public event CheckPulls CheckPulls;

        public CheckPullsHandler(ILogFactory logFactory) :
            // TODO: Sometimes, it is enough to hardcode the period right here, but sometimes it's better to move it to the settings.
            // Choose the simplest and sufficient solution
            base(TimeSpan.FromSeconds(CodeReviewTgBotSettings.TimeoutPeriodSeconds), logFactory)
        {
        }

        public override async Task Execute()
        {
            // TODO: Orchestrate execution flow here and delegate actual business logic implementation to services layer
            // Do not implement actual business logic here
            Console.WriteLine("IDBHPIBWSIPBPWS");

            CheckPulls?.Invoke();

            await Task.CompletedTask;
        }
    }
}
