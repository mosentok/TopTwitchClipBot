using System.Collections.Generic;

namespace TopTwitchClipBotModel
{
    public class PendingChannelConfigContainer
    {
        public decimal ChannelId { get; set; }
        public string Prefix { get; set; }
        public int? MinPostingHour { get; set; }
        public int? MaxPostingHour { get; set; }
        public int? NumberOfClipsAtATime { get; set; }
        public long? TimeSpanBetweenClipsAsTicks { get; set; }
        public int? GlobalMinViews { get; set; }
        public decimal? UtcHourOffset { get; set; }
        public string ClipOrder { get; set; }
        public List<PendingBroadcasterConfig> Broadcasters { get; set; }
        public PendingChannelConfigContainer FromBroadcasters(List<PendingBroadcasterConfig> broadcasters)
        {
            return new PendingChannelConfigContainer
            {
                ChannelId = ChannelId,
                Prefix = Prefix,
                MinPostingHour = MinPostingHour,
                MaxPostingHour = MaxPostingHour,
                NumberOfClipsAtATime = NumberOfClipsAtATime,
                TimeSpanBetweenClipsAsTicks = TimeSpanBetweenClipsAsTicks,
                GlobalMinViews = GlobalMinViews,
                UtcHourOffset = UtcHourOffset,
                ClipOrder = ClipOrder,
                Broadcasters = broadcasters
            };
        }
    }
}
