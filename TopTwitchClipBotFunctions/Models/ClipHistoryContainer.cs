using System;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotFunctions.Models
{
    public class ClipHistoryContainer
    {
        public decimal ChannelId { get; set; }
        public int BroadcasterConfigId { get; set; }
        public string Slug { get; set; }
        public string ClipUrl { get; set; }
        public string Title { get; set; }
        public int Views { get; set; }
        public float Duration { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime Stamp { get; set; }
        public ClipHistoryContainer() { }
        public ClipHistoryContainer(decimal channelId, BroadcasterHistoryContainer broadcasterHistory, string title, int views, float duration, DateTime createdAt)
        {
            ChannelId = channelId;
            BroadcasterConfigId = broadcasterHistory.BroadcasterConfigId;
            Slug = broadcasterHistory.Slug;
            ClipUrl = broadcasterHistory.ClipUrl;
            Stamp = broadcasterHistory.Stamp;
            Title = title;
            Views = views;
            Duration = duration;
            CreatedAt = createdAt;
        }
    }
}
