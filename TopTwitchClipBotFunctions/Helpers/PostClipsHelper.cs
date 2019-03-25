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
            var histories = channelContainer.Broadcasters.SelectMany(s => s.ExistingHistories);
            if (!histories.Any())
                return true;
            var latestHistoryStamp = histories.Max(s => s.Stamp);
            var timeSinceLastPost = now - latestHistoryStamp;
            return timeSinceLastPost.Ticks >= channelContainer.TimeSpanBetweenClipsAsTicks.Value;
        }
        public List<PendingChannelConfigContainer> ReadyToPostContainers(List<PendingChannelConfigContainer> channelContainers, DateTime yesterday, bool enableNumberOfClipsPerDay)
        {
            var pendingContainers = new List<PendingChannelConfigContainer>();
            var clipsCache = new Dictionary<string, GetClipsResponse>();
            foreach (var channelContainer in channelContainers)
            {
                var broadcasters = new List<PendingBroadcasterConfig>();
                foreach (var broadcasterContainer in channelContainer.Broadcasters)
                {
                    var isReadyToPost = IsReadyToPost();
                    if (isReadyToPost)
                        broadcasters.Add(broadcasterContainer);
                    bool IsReadyToPost()
                    {
                        if (!enableNumberOfClipsPerDay)
                            return true;
                        if (!broadcasterContainer.NumberOfClipsPerDay.HasValue) //no cap
                            return true;
                        var numberOfClipsSeenInTheLastDay = broadcasterContainer.ExistingHistories.Count(s => s.Stamp >= yesterday);
                        return numberOfClipsSeenInTheLastDay < broadcasterContainer.NumberOfClipsPerDay.Value; //is below cap
                    }
                }
                if (broadcasters.Any())
                    pendingContainers.Add(new PendingChannelConfigContainer(channelContainer, broadcasters));
            }
            return pendingContainers;
        }
        public List<PendingClipContainer> AtATimeContainers(List<ChannelClipsContainer> channelClipsContainers)
        {
            var clipContainers = new List<PendingClipContainer>();
            foreach (var channelClipsContainer in channelClipsContainers)
            {
                var takenClipContainers = TakeClipsAtATime();
                if (takenClipContainers.Any())
                    clipContainers.AddRange(takenClipContainers);
                List<PendingClipContainer> TakeClipsAtATime()
                {
                    var numberOfClipsAtATime = channelClipsContainer.PendingChannelConfigContainer.NumberOfClipsAtATime;
                    if (numberOfClipsAtATime.HasValue)
                        return channelClipsContainer.PendingClipContainers.Take(numberOfClipsAtATime.Value).ToList();
                    return channelClipsContainer.PendingClipContainers;
                }
            }
            return clipContainers;
        }
        public async Task<List<ChannelClipsContainer>> BuildClipContainers(string topClipsEndpoint, string clientId, string accept, List<PendingChannelConfigContainer> channelContainers)
        {
            var channelClipsContainers = new List<ChannelClipsContainer>();
            var clipsCache = new Dictionary<string, GetClipsResponse>();
            foreach (var channelContainer in channelContainers)
            {
                var pendingClipContainers = new List<PendingClipContainer>();
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
                var channelClipsContainer = new ChannelClipsContainer(channelContainer, pendingClipContainers);
                channelClipsContainers.Add(channelClipsContainer);
            }
            return channelClipsContainers;
        }
        public List<ChannelClipsContainer> ClipsWithMinViews(List<ChannelClipsContainer> channelClipsContainers)
        {
            var results = new List<ChannelClipsContainer>();
            foreach (var channelClipsContainer in channelClipsContainers)
            {
                var haveMinViews = new List<PendingClipContainer>();
                var globalMinViews = channelClipsContainer.PendingChannelConfigContainer.GlobalMinViews;
                foreach (var pendingClipContainer in channelClipsContainer.PendingClipContainers)
                {
                    if (pendingClipContainer.MinViews.HasValue)
                        TryAddResult(pendingClipContainer.MinViews.Value);
                    else if (globalMinViews.HasValue)
                        TryAddResult(globalMinViews.Value);
                    else
                        haveMinViews.Add(pendingClipContainer);
                    void TryAddResult(int minViews)
                    {
                        var clips = pendingClipContainer.Clips.Where(s => s.Views >= minViews).ToList();
                        if (clips.Any())
                        {
                            var hasMinViews = pendingClipContainer.FromClips(clips);
                            haveMinViews.Add(hasMinViews);
                        }
                    }
                }
                if (haveMinViews.Any())
                    results.Add(new ChannelClipsContainer(channelClipsContainer.PendingChannelConfigContainer, haveMinViews));
            }
            return results;
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
                //TODO add broadcaster to output
                var message = new StringBuilder()
                    .AppendLine($"**{insertedContainer.Title}**")
                    .Append($"**{insertedContainer.Views}** views, ")
                    .Append($"**{insertedContainer.Duration.ToString("N0")}s** long, ")
                    .AppendLine($"created at **{insertedContainer.CreatedAt} UTC**")
                    .Append(insertedContainer.ClipUrl)
                    .ToString();
                await channelContainer.Channel.SendMessageAsync(text: message);
            }
        }
    }
}
