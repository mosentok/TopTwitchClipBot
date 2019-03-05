using System;

namespace TopTwitchClipBotModel
{
    public class BroadcasterHistoryContainer
    {
        public decimal ChannelId { get; set; }
        public int BroadcasterConfigId { get; set; }
        public string Slug { get; set; }
        public string ClipUrl { get; set; }
        public DateTime Stamp { get; set; }
        public BroadcasterHistoryContainer() { }
        public BroadcasterHistoryContainer(decimal channelId, BroadcasterHistory broadcasterHistory)
        {
            ChannelId = channelId;
            BroadcasterConfigId = broadcasterHistory.BroadcasterConfigId;
            Slug = broadcasterHistory.Slug;
            ClipUrl = broadcasterHistory.ClipUrl;
            Stamp = broadcasterHistory.Stamp;
        }
    }
}
