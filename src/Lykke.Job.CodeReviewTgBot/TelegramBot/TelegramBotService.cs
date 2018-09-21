using Autofac;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Job.CodeReviewTgBot.AzureRepositories.PullRequests;
using Lykke.Job.CodeReviewTgBot.Core.Domain.PullRequests;
using Lykke.Job.CodeReviewTgBot.PeriodicalHandlers;
using Lykke.Job.CodeReviewTgBot.Settings.JobSettings;
using Lykke.Service.LykkeDevelopers.Client;
using Lykke.HttpClientGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Lykke.Job.CodeReviewTgBot.TelegramBot
{
    public class TelegramBotService : IStopable, IStartable
    {
        public static ILog _log;

        private readonly ITelegramBotClient _bot;
        private readonly IActivePullRequestsRepository _activePullRequestsRepository;
        private readonly CodeReviewTgBotSettings _settings;

        private static TelegramBotActions _actions;
        private static LykkeDevelopersClient _devClient;
        bool ServicseIsRunning = false;

        private CheckPullsHandler TimeToCheckPulls;

        public TelegramBotService(CodeReviewTgBotSettings settings, ILogFactory logFactory, IActivePullRequestsRepository activePullRequestsRepository)
        {
            _log = logFactory.CreateLog(this); 
            _activePullRequestsRepository = activePullRequestsRepository;
            _settings = settings;

            _bot = new TelegramBotClient(_settings.BotToken);
            _actions = new TelegramBotActions(_settings.OrgainzationName, _settings.GitToken, _settings.BotName);

            var httpClientGenerator = new HttpClientGeneratorBuilder(_settings.LykkeDevelopersServiceUrl);
            _devClient = new LykkeDevelopersClient(httpClientGenerator.Create());

            _bot.OnMessage += BotOnMessageReceived;
            _bot.OnMessageEdited += BotOnMessageReceived;

            TimeToCheckPulls = new CheckPullsHandler(logFactory);
            TimeToCheckPulls.CheckPulls += CheckPullRequests;
            TimeToCheckPulls.Start();


            //_bot.OnCallbackQuery += BotOnCallbackQueryReceived;
            //_bot.OnReceiveError += BotOnReceiveError;
        }
        private async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            //if (!ServicseIsRunning)
            //{
            //    ServicseIsRunning = true;
            //    await GetListOfPullRequests(messageEventArgs.Message.Chat.Id);
            //    ServicseIsRunning = false;
            //}

#if DEBUG
            Console.WriteLine("BotOnMessageReceived - " + messageEventArgs.Message.Text);
#endif

            var message = messageEventArgs.Message;

            if (message == null || message.Type != MessageType.Text) return;

            var firstWord = message.Text.Split(' ').First();
            var command = firstWord.IndexOf('@') == -1 ? firstWord : firstWord.Substring(0, firstWord.IndexOf('@'));

            if (command == "/groupId")
            {
                await SendTextToUser(message.Chat.Id, $"Group Id: {message.Chat.Id}");
                return;
            }

        }

        public void Start()
        {
            try
            {
                _log.Info("TelegramBotService.Start", "Start");

                var me = _bot.GetMeAsync().Result;
                Console.Title = me.Username;

                _bot.StartReceiving(Array.Empty<UpdateType>());
                _log.Info(nameof(TelegramBotService), nameof(TelegramBotService), $"Start listening for @{me.Username}");
            }
            catch (Exception ex)
            {
                _log.Error("TelegramBotService.Start", ex );
                return;
            }
        }

        public void Stop()
        {
            try
            {
                _log.Info("TelegramBotService.Stop", "Stop");

                _bot.StopReceiving();
                _log.Info(nameof(TelegramBotService), nameof(TelegramBotService), "Stop listening.");
            }
            catch (Exception ex)
            {
                _log.Error("TelegramBotService.Stop",ex);
                return;
            }
        }

        public void Dispose()
        {
            TimeToCheckPulls.CheckPulls -= CheckPullRequests;
            Dispose();
        }

        private async Task SendTextToUser(long chatId, string text = "")
        {
            const string usage = @"
Usage:
/addGit - to start creating repository
/resetMyTeam - to reset your team";

            await _bot.SendTextMessageAsync(
                chatId,
                String.IsNullOrWhiteSpace(text) ? usage : text,
                replyMarkup: new ReplyKeyboardRemove());
        }

        private async Task GetListOfPullRequests(long chatId)
        {
            var activePulls = new List<ActivePullRequest>();
            var allActivePulls = new List<ActivePullRequest>();

            var reposInOrganisation = await _actions.GetReposInOrganisation();

            var oldActivePulls = await _activePullRequestsRepository.GetAllAsync();            

            _log.Info("Checking pull requests...");

            foreach (var repo in reposInOrganisation)
            {
                var pulls = await _actions.GetPullsForRepo(repo.Id);
                
                foreach (var pull in pulls)
                {
                    var lastPull = oldActivePulls.FirstOrDefault(p => p.RowKey == pull.Id.ToString());
                    var pullToAdd = new ActivePullRequest() { RowKey = pull.Id.ToString(), PullRequestName = pull.Title, PullRequestUrl = pull.Url };
                    allActivePulls.Add(pullToAdd);
                    //if (lastPull != null && CheckTotalTimeLimit(lastPull.Timestamp.UtcDateTime))
                    //{
                    //    continue;
                    //}
                       
                    var message = repo.Name + " _ " + pull.Title + '\n';                    
                    //message += "ID:" + pull.Id + '\n';                    
                    var users = await _actions.GetUsersForRepo(repo.Id);
                    foreach (var user in users)
                    {
                        if (!message.Contains('@' + user.Login + '\n'))
                        {
                           
                            var dev = await _devClient.Developer.GetDeveloperByGitAcc(user.Login);

                            if (dev != null && !String.IsNullOrEmpty(dev.TelegramAcc))
                            {
                                message += '@' + dev.TelegramAcc + '\n';
                            }
                            else
                            {
                                message += "GitHub user with login @" + user.Login + '\n';
                            }
                            
                        }
                    }

                    message += pull.HtmlUrl + '\n';
                    await SendTextToUser(chatId, message);
                    //Console.WriteLine(message);
                    activePulls.Add(pullToAdd);
                }
            }

            var activePullsToRemove = oldActivePulls.Where(x => allActivePulls.All(y => y.RowKey != x.RowKey));

            foreach (var pullToRemove in activePullsToRemove)
            {
                await _activePullRequestsRepository.RemoveAsync(pullToRemove.RowKey);
                Console.WriteLine("Removed " + pullToRemove.RowKey);
            }
            
            await _activePullRequestsRepository.SaveRangeAsync(activePulls);

            _log.Info("Finish checking.");
        }

        private bool CheckTotalTimeLimit(DateTime dateTime)
        {
            var exactTimeNow = DateTime.Now.ToUniversalTime();
            var timeSpan = exactTimeNow.Subtract(dateTime);
            if (timeSpan.TotalMinutes > _settings.TotalTimeLimitInMinutes)
            {
                return false;
            }
            return true;
        }

        private async void CheckPullRequests()
        {
            if(_settings.AllowedGroupId != 0)
                await GetListOfPullRequests(_settings.AllowedGroupId);
        }

    }
}
