﻿using System;
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
        readonly ITwitchWrapper _TwitchWrapper;
        readonly IDiscordWrapper _DiscordWrapper;
        readonly ITopTwitchClipBotContext _Context;
        readonly ILoggerWrapper _Log;
        public PostClipsHelper(ILoggerWrapper log, ITopTwitchClipBotContext context, ITwitchWrapper twitchWrapper, IDiscordWrapper discordWrapper)
        {
            _TwitchWrapper = twitchWrapper;
            _DiscordWrapper = discordWrapper;
            _Context = context;
            _Log = log;
        }
        public bool IsReadyToPost(int? numberOfClipsPerDay, List<BroadcasterHistoryContainer> existingHistories, DateTime yesterday)
        {
            if (!numberOfClipsPerDay.HasValue) //no cap
                return true;
            var numberOfClipsSeenInTheLastDay = existingHistories.Count(s => s.Stamp >= yesterday);
            return numberOfClipsSeenInTheLastDay < numberOfClipsPerDay.Value; //is below cap
        }
        public async Task<List<PendingClipContainer>> BuildClipContainers(string topClipsEndpoint, string clientId, string accept, List<PendingBroadcasterConfig> containers)
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
                    var response = await _TwitchWrapper.GetClips(channelEndpoint, clientId, accept);
                    var pendingClipContainer = new PendingClipContainer(container, response);
                    pendingClipContainers.Add(pendingClipContainer);
                    clipsCache.Add(container.Broadcaster, response);
                }
            return pendingClipContainers;
        }
        public async Task<List<BroadcasterHistoryContainer>> InsertHistories(List<PendingClipContainer> pendingClipContainers)
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
            return await _Context.InsertBroadcasterHistoriesAsync(historyContainers);
        }
        public async Task<List<ChannelContainer>> BuildChannelContainers(List<BroadcasterHistoryContainer> insertedHistories)
        {
            var groupedHistories = insertedHistories.ToLookup(s => s.ChannelId);
            var channelContainers = new List<ChannelContainer>();
            foreach (var historyGroup in groupedHistories)
            {
                var channel = await _DiscordWrapper.GetChannelAsync(historyGroup.Key);
                var channelContainer = new ChannelContainer(historyGroup.ToList(), channel);
                channelContainers.Add(channelContainer);
            }
            return channelContainers;
        }
    }
}
