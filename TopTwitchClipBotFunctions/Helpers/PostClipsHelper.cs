using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TopTwitchClipBotFunctions.Models;
using TopTwitchClipBotFunctions.Wrappers;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotFunctions.Helpers
{
    public class PostClipsHelper
    {
        public ITwitchWrapper TwitchWrapper { get; set; }
        public IDiscordWrapper DiscordWrapper { get; set; }
        public ITopTwitchClipBotContext Context { get; set; }
        public PostClipsHelper(ITwitchWrapper twitchWrapper, IDiscordWrapper discordWrapper, ITopTwitchClipBotContext context)
        {
            TwitchWrapper = twitchWrapper;
            DiscordWrapper = discordWrapper;
            Context = context;
        }
        public async Task PostClips(string topClipsEndpoint, string clientId, string accept, DateTime yesterday)
        {
            await DiscordWrapper.LogInAsync();
            var containers = await Context.GetChannelTopClipConfigsAsync();
            var containersReadyToPost = containers.Where(s => IsReadyToPost(s, yesterday)).ToList();
            var pendingClipContainers = await BuildClipContainers(topClipsEndpoint, clientId, accept, containersReadyToPost);
            var insertedHistories = await InsertHistories(pendingClipContainers);
            var channelContainers = await BuildChannelContainers(insertedHistories);
            foreach (var channelContainer in channelContainers)
                foreach (var history in channelContainer.TopClipHistoryContainers)
                    await channelContainer.Channel.SendMessageAsync(history.ClipUrl);
            await DiscordWrapper.LogOutAsync();
        }
        bool IsReadyToPost(PendingChannelTopClipConfig config, DateTime yesterday)
        {
            var hasNoDailyCap = !config.NumberOfClipsPerDay.HasValue;
            var isBelowDailyCap = config.ExistingHistories.Count(s => s.Stamp >= yesterday) < config.NumberOfClipsPerDay.Value;
            return hasNoDailyCap || isBelowDailyCap;
        }
        async Task<List<PendingClipContainer>> BuildClipContainers(string topClipsEndpoint, string clientId, string accept, List<PendingChannelTopClipConfig> containers)
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
                    var response = await TwitchWrapper.GetClips(channelEndpoint, clientId, accept);
                    var pendingClipContainer = new PendingClipContainer(container, response);
                    pendingClipContainers.Add(pendingClipContainer);
                    clipsCache.Add(container.Broadcaster, response);
                }
            return pendingClipContainers;
        }
        async Task<List<TopClipHistoryContainer>> InsertHistories(List<PendingClipContainer> pendingClipContainers)
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
            return await Context.InsertTopClipHistoriesAsync(historyContainers);
        }
        async Task<List<ChannelContainer>> BuildChannelContainers(List<TopClipHistoryContainer> insertedHistories)
        {
            var groupedHistories = insertedHistories.ToLookup(s => s.ChannelId);
            var channelContainers = new List<ChannelContainer>();
            foreach (var historyGroup in groupedHistories)
            {
                var channel = await DiscordWrapper.GetChannelAsync(historyGroup.Key);
                var channelContainer = new ChannelContainer(historyGroup.ToList(), channel);
                channelContainers.Add(channelContainer);
            }
            return channelContainers;
        }
    }
}
