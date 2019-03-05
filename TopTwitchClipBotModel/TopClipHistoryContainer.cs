using System;

namespace TopTwitchClipBotModel
{
    public class TopClipHistoryContainer
    {
        public decimal ChannelId { get; set; }
        public int ChannelTopClipConfigId { get; set; }
        public string Slug { get; set; }
        public string ClipUrl { get; set; }
        public DateTime Stamp { get; set; }
        public TopClipHistoryContainer() { }
        public TopClipHistoryContainer(decimal channelId, TopClipHistory topClipHistory)
        {
            ChannelId = channelId;
            ChannelTopClipConfigId = topClipHistory.ChannelTopClipConfigId;
            Slug = topClipHistory.Slug;
            ClipUrl = topClipHistory.ClipUrl;
            Stamp = topClipHistory.Stamp;
        }
    }
}
