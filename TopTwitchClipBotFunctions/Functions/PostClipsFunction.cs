using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using TopTwitchClipBotFunctions.Models;
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
            using (var twitchWrapper = new TwitchWrapper())
            using (var discordWrapper = new DiscordWrapper(botToken))
            using (var context = new TopTwitchClipBotContext(connectionString))
                await PostClips(topClipsEndpoint, clientId, accept, yesterday, twitchWrapper, discordWrapper, context);
        }
        static async Task PostClips(string topClipsEndpoint, string clientId, string accept, DateTime yesterday, TwitchWrapper twitchWrapper, DiscordWrapper discordWrapper, TopTwitchClipBotContext context)
        {
            await discordWrapper.LogInAsync();
            var containers = await context.GetChannelTopClipConfigsAsync();
            var containersReadyToPost = containers.Where(s => IsReadyToPost(s, yesterday)).ToList();
            var pendingClipContainers = await BuildClipContainers(topClipsEndpoint, clientId, accept, twitchWrapper, containersReadyToPost);
            var insertedHistories = await InsertHistories(context, pendingClipContainers);
            var channelContainers = await BuildChannelContainers(discordWrapper, insertedHistories);
            foreach (var channelContainer in channelContainers)
                foreach (var history in channelContainer.TopClipHistoryContainers)
                    await channelContainer.Channel.SendMessageAsync(history.ClipUrl);
            await discordWrapper.LogOutAsync();
        }
        static bool IsReadyToPost(PendingChannelTopClipConfig config, DateTime yesterday)
        {
            var hasNoDailyCap = !config.NumberOfClipsPerDay.HasValue;
            var isBelowDailyCap = config.ExistingHistories.Count(s => s.Stamp >= yesterday) < config.NumberOfClipsPerDay.Value;
            return hasNoDailyCap || isBelowDailyCap;
        }
        static async Task<List<PendingClipContainer>> BuildClipContainers(string topClipsEndpoint, string clientId, string accept, TwitchWrapper twitchWrapper, List<PendingChannelTopClipConfig> containers)
        {
            var pendingClipContainers = new List<PendingClipContainer>();
            var clipsCache = new Dictionary<string, GetClipsResponse>();
            foreach (var container in containers)
                if (clipsCache.ContainsKey(container.Broadcaster))
                {
                    var pendingClipContainer = new PendingClipContainer(container, clipsCache[container.Broadcaster]);
                    pendingClipContainers.Add(pendingClipContainer);
                }
                else
                {
                    var channelEndpoint = $"{topClipsEndpoint}&channel={container.Broadcaster}";
                    var response = await twitchWrapper.GetClips(channelEndpoint, clientId, accept);
                    var pendingClipContainer = new PendingClipContainer(container, response);
                    pendingClipContainers.Add(pendingClipContainer);
                    clipsCache.Add(container.Broadcaster, response);
                }
            return pendingClipContainers;
        }
        static async Task<List<TopClipHistoryContainer>> InsertHistories(TopTwitchClipBotContext context, List<PendingClipContainer> pendingClipContainers)
        {
            var historyContainers = new List<TopClipHistoryContainer>();
            foreach (var clipContainer in pendingClipContainers)
            {
                var firstUnseenClip = clipContainer.GetClipsResponse.Clips.FirstOrDefault(t => !clipContainer.PendingChannelTopClipConfig.ExistingHistories.Any(u => u.Slug == t.Slug));
                if (firstUnseenClip != null)
                {
                    var historyContainer = new TopClipHistoryContainer
                    {
                        ChannelId = clipContainer.PendingChannelTopClipConfig.ChannelId,
                        ChannelTopClipConfigId = clipContainer.PendingChannelTopClipConfig.Id,
                        Slug = firstUnseenClip.Slug,
                        ClipUrl = firstUnseenClip.Url,
                        Stamp = DateTime.Now
                    };
                    historyContainers.Add(historyContainer);
                }
            }
            return await context.InsertTopClipHistoriesAsync(historyContainers);
        }
        static async Task<List<ChannelContainer>> BuildChannelContainers(DiscordWrapper discordWrapper, List<TopClipHistoryContainer> insertedHistories)
        {
            var groupedHistories = insertedHistories.ToLookup(s => s.ChannelId);
            var channelContainers = new List<ChannelContainer>();
            foreach (var historyGroup in groupedHistories)
            {
                var channel = await discordWrapper.GetChannelAsync(historyGroup.Key);
                var channelContainer = new ChannelContainer(historyGroup.ToList(), channel);
                channelContainers.Add(channelContainer);
            }
            return channelContainers;
        }
    }
}
