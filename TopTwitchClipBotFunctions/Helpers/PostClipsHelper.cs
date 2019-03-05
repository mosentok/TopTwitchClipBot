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
            var containers = await Context.GetBroadcasterConfigsAsync();
            var containersReadyToPost = containers.Where(s => IsReadyToPost(s, yesterday)).ToList();
            var pendingClipContainers = await BuildClipContainers(topClipsEndpoint, clientId, accept, containersReadyToPost);
            var insertedHistories = await InsertHistories(pendingClipContainers);
            var channelContainers = await BuildChannelContainers(insertedHistories);
            foreach (var channelContainer in channelContainers)
                foreach (var history in channelContainer.BroadcasterHistoryContainers)
                    await channelContainer.Channel.SendMessageAsync(history.ClipUrl);
            await DiscordWrapper.LogOutAsync();
        }
        bool IsReadyToPost(PendingBroadcasterConfig config, DateTime yesterday)
        {
            return !config.NumberOfClipsPerDay.HasValue || //no cap, or
                config.ExistingHistories.Count(s => s.Stamp >= yesterday) < config.NumberOfClipsPerDay; //is below cap
        }
        async Task<List<PendingClipContainer>> BuildClipContainers(string topClipsEndpoint, string clientId, string accept, List<PendingBroadcasterConfig> containers)
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
        async Task<List<BroadcasterHistoryContainer>> InsertHistories(List<PendingClipContainer> pendingClipContainers)
        {
            var historyContainers = new List<BroadcasterHistoryContainer>();
            foreach (var clipContainer in pendingClipContainers)
            {
                var firstUnseenClip = clipContainer.GetClipsResponse.Clips.FirstOrDefault(t => !clipContainer.PendingBroadcasterConfig.ExistingHistories.Any(u => u.Slug == t.Slug));
                if (firstUnseenClip != null)
                {
                    var historyContainer = new BroadcasterHistoryContainer
                    {
                        ChannelId = clipContainer.PendingBroadcasterConfig.ChannelId,
                        BroadcasterConfigId = clipContainer.PendingBroadcasterConfig.Id,
                        Slug = firstUnseenClip.Slug,
                        ClipUrl = firstUnseenClip.Url,
                        Stamp = DateTime.Now
                    };
                    historyContainers.Add(historyContainer);
                }
            }
            return await Context.InsertBroadcasterHistoriesAsync(historyContainers);
        }
        async Task<List<ChannelContainer>> BuildChannelContainers(List<BroadcasterHistoryContainer> insertedHistories)
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
