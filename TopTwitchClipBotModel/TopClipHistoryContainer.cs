using System;

namespace TopTwitchClipBotModel
{
    public class TopClipHistoryContainer
    {
        public decimal ChannelId { get; set; }
        public string Slug { get; set; }
        public string ClipUrl { get; set; }
        public DateTime Stamp { get; set; }
        public TopClipHistoryContainer()        {        }
        public TopClipHistoryContainer(TopClipHistory topClipHistory)
        {
            ChannelId = topClipHistory.ChannelId;
            Slug = topClipHistory.Slug;
            ClipUrl = topClipHistory.ClipUrl;
            Stamp = topClipHistory.Stamp;
        }
    }
}
