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
            using (var twitchWrapper = new TwitchWrapper())
            using (var discordWrapper = new DiscordWrapper(botToken))
            using (var context = new TopTwitchClipBotContext(connectionString))
            {
                await discordWrapper.LogInAsync();
                var containers = await context.GetChannelTopClipConfigsAsync();
                var clips = await BuildClipContainers(topClipsEndpoint, clientId, accept, twitchWrapper, containers);
                var insertedHistories = await InsertHistories(context, clips);
                var channels = await BuildChannelContainers(discordWrapper, insertedHistories);
                foreach (var channel in channels)
                    await channel.Channel.SendMessageAsync(channel.TopClipHistoryContainer.ClipUrl);
                await discordWrapper.LogOutAsync();
            }
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
        static async Task<List<TopClipHistoryContainer>> InsertHistories(TopTwitchClipBotContext context, List<PendingClipContainer> clipDictionary)
        {
            var historyContainers = new List<TopClipHistoryContainer>();
            foreach (var clipContainer in clipDictionary)
            {
                var firstUnseenClip = clipContainer.GetClipsResponse.Clips.FirstOrDefault(t => !clipContainer.PendingChannelTopClipConfig.ExistingSlugs.Contains(t.Slug));
                if (firstUnseenClip != null)
                {
                    var historyContainer = new TopClipHistoryContainer
                    {
                        ChannelId = clipContainer.PendingChannelTopClipConfig.ChannelId,
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
            var channelContainers = new List<ChannelContainer>();
            foreach (var history in insertedHistories)
            {
                var channel = await discordWrapper.GetChannelAsync(history.ChannelId);
                var channelContainer = new ChannelContainer(history, channel);
                channelContainers.Add(channelContainer);
            }
            return channelContainers;
        }
    }
}
