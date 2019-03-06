using System;
using System.Linq;
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
            var yesterday = DateTime.Now.AddDays(-1);
            var logWrapper = new LoggerWrapper(log);
            using (var twitchWrapper = new TwitchWrapper())
            using (var discordWrapper = new DiscordWrapper(botToken))
            using (var context = new TopTwitchClipBotContext(connectionString))
            {
                var helper = new PostClipsHelper(logWrapper, context, twitchWrapper, discordWrapper);
                logWrapper.LogInformation("Posting clips.");
                await discordWrapper.LogInAsync();
                var containers = await context.GetBroadcasterConfigsAsync();
                var containersReadyToPost = containers.Where(s => helper.IsReadyToPost(s.NumberOfClipsPerDay, s.ExistingHistories, yesterday)).ToList();
                var pendingClipContainers = await helper.BuildClipContainers(topClipsEndpoint, clientId, accept, containersReadyToPost);
                var insertedHistories = await helper.InsertHistories(pendingClipContainers);
                var channelContainers = await helper.BuildChannelContainers(insertedHistories);
                foreach (var channelContainer in channelContainers)
                    foreach (var history in channelContainer.BroadcasterHistoryContainers)
                        await channelContainer.Channel.SendMessageAsync(history.ClipUrl);
                await discordWrapper.LogOutAsync();
                logWrapper.LogInformation("Posted clips.");
            }
        }
    }
}
