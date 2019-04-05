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
            var histories = channelContainer.Broadcasters.SelectMany(s => s.ExistingHistories).ToList();
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
                var broadcastersReadyToPost = new List<PendingBroadcasterConfig>();
                foreach (var broadcasterContainer in channelContainer.Broadcasters)
                {
                    var isReadyToPost = IsReadyToPost();
                    if (isReadyToPost)
                        broadcastersReadyToPost.Add(broadcasterContainer);
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
                if (broadcastersReadyToPost.Any())
                    pendingContainers.Add(channelContainer.FromBroadcasters(broadcastersReadyToPost));
            }
            return pendingContainers;
        }
        public List<UnseenChannelClipsContainer> AtATimeContainers(List<UnseenChannelClipsContainer> unseenChannelClipsContainers)
        {
            var results = new List<UnseenChannelClipsContainer>();
            foreach (var unseenChannelClipsContainer in unseenChannelClipsContainers)
            {
                var takenClipContainers = TakeClipsAtATime();
                if (takenClipContainers.UnseenClips.Any())
                    results.Add(takenClipContainers);
                UnseenChannelClipsContainer TakeClipsAtATime()
                {
                    var pendingChannelConfigContainer = unseenChannelClipsContainer.PendingChannelConfigContainer;
                    var numberOfClipsAtATime = pendingChannelConfigContainer.NumberOfClipsAtATime;
                    if (!numberOfClipsAtATime.HasValue)
                        return unseenChannelClipsContainer;
                    var ordered = OrderUnseenClips();
                    var takenAtATime = ordered.Take(numberOfClipsAtATime.Value).ToList();
                    return new UnseenChannelClipsContainer(pendingChannelConfigContainer, takenAtATime);
                    IOrderedEnumerable<ClipHistoryContainer> OrderUnseenClips()
                    {
                        var clipOrder = unseenChannelClipsContainer.PendingChannelConfigContainer.ClipOrder;
                        if (string.IsNullOrEmpty(clipOrder))
                            return unseenChannelClipsContainer.UnseenClips.OrderBy(s => s.BroadcasterLastSeenAt);
                        switch (clipOrder.ToLower())
                        {
                            //TODO replace with config
                            case "views":
                            case "view count":
                                return unseenChannelClipsContainer.UnseenClips.OrderByDescending(s => s.Views);
                            case "oldest first":
                            case "oldest":
                            case "even mix":
                            default:
                                return unseenChannelClipsContainer.UnseenClips.OrderBy(s => s.BroadcasterLastSeenAt);
                        }
                    }
                }
            }
            return results;
        }
        public List<UnseenChannelClipsContainer> BuildUnseenClipContainers(List<ChannelClipsContainer> channelClipsContainers)
        {
            var results = new List<UnseenChannelClipsContainer>();
            foreach (var channelClipsContainer in channelClipsContainers)
            {
                var unseenClips = new List<ClipHistoryContainer>();
                foreach (var clipContainer in channelClipsContainer.PendingClipContainers)
                {
                    var firstUnseenClip = clipContainer.Clips.FirstOrDefault(t => !clipContainer.ExistingHistories.Any(u => u.Slug == t.Slug));
                    if (firstUnseenClip != null)
                    {
                        var broadcasterLastSeenAt = BroadcasterLastSeenAt();
                        var historyContainer = new ClipHistoryContainer
                        {
                            ChannelId = clipContainer.ChannelId,
                            BroadcasterConfigId = clipContainer.Id,
                            BroadcasterLastSeenAt = broadcasterLastSeenAt,
                            Slug = firstUnseenClip.Slug,
                            ClipUrl = firstUnseenClip.Url,
                            Stamp = DateTime.Now,
                            CreatedAt = firstUnseenClip.CreatedAt,
                            Duration = firstUnseenClip.Duration,
                            Title = firstUnseenClip.Title,
                            Views = firstUnseenClip.Views
                        };
                        unseenClips.Add(historyContainer);
                    }
                    DateTime? BroadcasterLastSeenAt()
                    {
                        if (!clipContainer.ExistingHistories.Any())
                            return null;
                        return clipContainer.ExistingHistories.Max(s => s.Stamp);
                }
                }
                if (unseenClips.Any())
                    results.Add(new UnseenChannelClipsContainer(channelClipsContainer.PendingChannelConfigContainer, unseenClips));
            }
            return results;
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
        public async Task InsertHistories(List<ClipHistoryContainer> inserted)
        {
            var toInsert = inserted.Select(s => new BroadcasterHistoryContainer
            {
                BroadcasterConfigId = s.BroadcasterConfigId,
                ChannelId = s.ChannelId,
                ClipUrl = s.ClipUrl,
                Slug = s.Slug,
                Stamp = s.Stamp
            }).ToList();
            await _Context.InsertBroadcasterHistoriesAsync(toInsert);
        }
        public async Task<List<ChannelContainer>> BuildChannelContainers(List<UnseenChannelClipsContainer> clipContainers)
        {
            var channelContainers = new List<ChannelContainer>();
            foreach (var clipContainer in clipContainers)
            {
                var channel = await _DiscordWrapper.GetChannelAsync(clipContainer.PendingChannelConfigContainer.ChannelId);
                var channelContainer = new ChannelContainer(clipContainer, channel);
                channelContainers.Add(channelContainer);
            }
            return channelContainers;
        }
        public async Task SendMessagesAsync(ChannelContainer channelContainer)
        {
            var unseenChannelClipsContainer = channelContainer.UnseenChannelClipsContainer;
            foreach (var insertedContainer in unseenChannelClipsContainer.UnseenClips)
            {
                var createdAtString = BuildCreatedAtString();
                //TODO add broadcaster to output
                var message = new StringBuilder()
                    .AppendLine($"**{insertedContainer.Title}**")
                    .Append($"**{insertedContainer.Views}** views, ")
                    .Append($"**{insertedContainer.Duration.ToString("N0")}s** long, ")
                    .AppendLine(createdAtString)
                    .Append(insertedContainer.ClipUrl)
                    .ToString();
                //TODO try/catch
                await channelContainer.Channel.SendMessageAsync(text: message);
                string BuildCreatedAtString()
                {
                    var utcHourOffset = channelContainer.UnseenChannelClipsContainer.PendingChannelConfigContainer.UtcHourOffset;
                    if (!utcHourOffset.HasValue)
                        return $"created at **{insertedContainer.CreatedAt} UTC**";
                    var utcHourOffsetDouble = Convert.ToDouble(utcHourOffset.Value);
                    var convertedToTimeZone = insertedContainer.CreatedAt.AddHours(utcHourOffsetDouble);
                    return $"created at **{insertedContainer.CreatedAt}**";
                }
            }
        }
    }
}
