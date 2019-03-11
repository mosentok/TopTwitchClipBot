using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public List<PendingChannelConfigContainer> AfterTimeBetweenClips(List<PendingChannelConfigContainer> channelContainers, DateTime now)
        {
            var pendingContainers = new List<PendingChannelConfigContainer>();
            foreach (var channelContainer in channelContainers)
            {
                var isAfterTimeBetweenClips = IsAfterTimeBetweenClips(channelContainer, now);
                if (isAfterTimeBetweenClips)
                    pendingContainers.Add(channelContainer);
            }
            return pendingContainers;
        }
        static bool IsAfterTimeBetweenClips(PendingChannelConfigContainer channelContainer, DateTime now)
        {
            if (!channelContainer.TimeSpanBetweenClipsAsTicks.HasValue)
                return true;
            var latestHistoryStamp = channelContainer.Broadcasters.SelectMany(s => s.ExistingHistories).Max(s => s.Stamp);
            var timeSinceLastPost = now - latestHistoryStamp;
            return timeSinceLastPost.Ticks >= channelContainer.TimeSpanBetweenClipsAsTicks.Value;
        }
        public List<PendingChannelConfigContainer> ReadyToPostContainers(List<PendingChannelConfigContainer> channelContainers, DateTime yesterday)
        {
            var pendingContainers = new List<PendingChannelConfigContainer>();
            var clipsCache = new Dictionary<string, GetClipsResponse>();
            foreach (var channelContainer in channelContainers)
            {
                var broadcasters = new List<PendingBroadcasterConfig>();
                foreach (var broadcasterContainer in channelContainer.Broadcasters)
                {
                    var isReadyToPost = IsReadyToPost(broadcasterContainer, yesterday);
                    if (isReadyToPost)
                        broadcasters.Add(broadcasterContainer);
                }
                if (broadcasters.Any())
                    pendingContainers.Add(new PendingChannelConfigContainer(channelContainer, broadcasters));
            }
            return pendingContainers;
        }
        static bool IsReadyToPost(PendingBroadcasterConfig pendingBroadcaster, DateTime yesterday)
        {
            if (!pendingBroadcaster.NumberOfClipsPerDay.HasValue) //no cap
                return true;
            var numberOfClipsSeenInTheLastDay = pendingBroadcaster.ExistingHistories.Count(s => s.Stamp >= yesterday);
            return numberOfClipsSeenInTheLastDay < pendingBroadcaster.NumberOfClipsPerDay.Value; //is below cap
        }
        public List<PendingChannelConfigContainer> AtATimeContainers(List<PendingChannelConfigContainer> channelContainers)
        {
            var pendingContainers = new List<PendingChannelConfigContainer>();
            var clipsCache = new Dictionary<string, GetClipsResponse>();
            foreach (var channelContainer in channelContainers)
            {
                var broadcasters = GetBroadcastersToPost(channelContainer);
                if (broadcasters.Any())
                    pendingContainers.Add(new PendingChannelConfigContainer(channelContainer, broadcasters));
            }
            return pendingContainers;
        }
        static List<PendingBroadcasterConfig> GetBroadcastersToPost(PendingChannelConfigContainer channelContainer)
        {
            if (channelContainer.NumberOfClipsAtATime.HasValue)
                return channelContainer.Broadcasters.Take(channelContainer.NumberOfClipsAtATime.Value).ToList();
            return channelContainer.Broadcasters;
        }
        public async Task<List<PendingClipContainer>> BuildClipContainers(string topClipsEndpoint, string clientId, string accept, List<PendingChannelConfigContainer> channelContainers)
        {
            var pendingClipContainers = new List<PendingClipContainer>();
            var clipsCache = new Dictionary<string, GetClipsResponse>();
            foreach (var channelContainer in channelContainers)
                foreach (var broadcasterContainer in channelContainer.Broadcasters)
                    if (clipsCache.ContainsKey(broadcasterContainer.Broadcaster))
                    {
                        var pendingClipContainer = new PendingClipContainer(broadcasterContainer, clipsCache[broadcasterContainer.Broadcaster]);
                        pendingClipContainers.Add(pendingClipContainer);
                    }
                    else
                    {
                        var channelEndpoint = $"{topClipsEndpoint}&channel={broadcasterContainer.Broadcaster}";
                        var response = await _TwitchWrapper.GetClips(channelEndpoint, clientId, accept);
                        var pendingClipContainer = new PendingClipContainer(broadcasterContainer, response);
                        pendingClipContainers.Add(pendingClipContainer);
                        clipsCache.Add(broadcasterContainer.Broadcaster, response);
                    }
            return pendingClipContainers;
        }
        public async Task<List<InsertedBroadcasterHistoryContainer>> InsertHistories(List<PendingClipContainer> pendingClipContainers)
        {
            var historyContainers = new List<BroadcasterHistoryContainer>();
            var inserted = new List<InsertedBroadcasterHistoryContainer>();
            foreach (var clipContainer in pendingClipContainers)
            {
                var firstUnseenClip = clipContainer.Clips.FirstOrDefault(t => !clipContainer.ExistingHistories.Any(u => u.Slug == t.Slug));
                if (firstUnseenClip != null)
                {
                    var historyContainer = new BroadcasterHistoryContainer
                    {
                        ChannelId = clipContainer.ChannelId,
                        BroadcasterConfigId = clipContainer.Id,
                        Slug = firstUnseenClip.Slug,
                        ClipUrl = firstUnseenClip.Url,
                        Stamp = DateTime.Now
                    };
                    historyContainers.Add(historyContainer);
                    var insertedContainer = new InsertedBroadcasterHistoryContainer(clipContainer.ChannelId, historyContainer, firstUnseenClip.Title, firstUnseenClip.Views, firstUnseenClip.Duration, firstUnseenClip.CreatedAt);
                    inserted.Add(insertedContainer);
                }
            }
            await _Context.InsertBroadcasterHistoriesAsync(historyContainers);
            return inserted;
        }
        public async Task<List<ChannelContainer>> BuildChannelContainers(List<InsertedBroadcasterHistoryContainer> insertedHistories)
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
        public async Task SendMessagesAsync(ChannelContainer channelContainer)
        {
            foreach (var insertedContainer in channelContainer.Inserted)
            {
                var message = new StringBuilder()
                    .AppendLine($"**Title** {insertedContainer.Title}")
                    .AppendLine($"**Views** {insertedContainer.Views}")
                    .AppendLine($"**Duration** {insertedContainer.Duration.ToString("N2")}")
                    .AppendLine($"**Created at** {insertedContainer.CreatedAt} UTC")
                    .Append(insertedContainer.ClipUrl)
                    .ToString();
                await channelContainer.Channel.SendMessageAsync(text: message);
            }
        }
    }
}
