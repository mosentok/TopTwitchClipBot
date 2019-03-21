using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using TopTwitchClipBotFunctions.Helpers;
using TopTwitchClipBotFunctions.Wrappers;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotFunctions.Functions
{
    public static class PostClipsFunction
    {
        [FunctionName(nameof(PostClipsFunction))]
        public static async Task Run([TimerTrigger("%PostClipsFunctionCron%")]TimerInfo myTimer, ILogger log)
        {
            var topClipsEndpoint = Environment.GetEnvironmentVariable("TwitchTopClipsEndpoint");
            var clientId = Environment.GetEnvironmentVariable("TwitchClientId");
            var accept = Environment.GetEnvironmentVariable("TwitchAcceptHeaderValue");
            var botToken = Environment.GetEnvironmentVariable("BotToken");
            var connectionString = Environment.GetEnvironmentVariable("TopTwitchClipBotConnectionString");
            var now = DateTime.Now;
            var yesterday = now.AddDays(-1);
            var logWrapper = new LoggerWrapper(log);
            using (var twitchWrapper = new TwitchWrapper())
            using (var discordWrapper = new DiscordWrapper(botToken))
            using (var context = new TopTwitchClipBotContext(connectionString))
            {
                var helper = new PostClipsHelper(logWrapper, context, twitchWrapper, discordWrapper);
                logWrapper.LogInformation("Posting clips.");
                await discordWrapper.LogInAsync();
                var containers = await context.GetPendingChannelConfigsAsync(now.Hour);
                var afterTimeBetweenClipsContainers = helper.AfterTimeBetweenClips(containers, now);
                var readyToPostContainers = helper.ReadyToPostContainers(afterTimeBetweenClipsContainers, yesterday);
                //TODO need to move "at a time" lower, because it's filtering out broadcasters that could have otherwise posted
                var atATimeContainers = helper.AtATimeContainers(readyToPostContainers);
                var pendingClipContainers = await helper.BuildClipContainers(topClipsEndpoint, clientId, accept, atATimeContainers);
                var clipsWithMinViews = helper.ClipsWithMinViews(pendingClipContainers);
                var inserted = await helper.InsertHistories(clipsWithMinViews);
                var channelContainers = await helper.BuildChannelContainers(inserted);
                foreach (var channelContainer in channelContainers)
                    await helper.SendMessagesAsync(channelContainer);
                await discordWrapper.LogOutAsync();
                logWrapper.LogInformation("Posted clips.");
            }
        }
    }
}
