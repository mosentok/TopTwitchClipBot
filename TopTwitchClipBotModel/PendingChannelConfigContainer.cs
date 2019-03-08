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
        public List<PendingBroadcasterConfig> Broadcasters { get; set; }
        public PendingChannelConfigContainer() { }
        public PendingChannelConfigContainer(PendingChannelConfigContainer channelContainer, List<PendingBroadcasterConfig> broadcasters)
        {
            ChannelId = channelContainer.ChannelId;
            Prefix = channelContainer.Prefix;
            MinPostingHour = channelContainer.MinPostingHour;
            MaxPostingHour = channelContainer.MaxPostingHour;
            NumberOfClipsAtATime = channelContainer.NumberOfClipsAtATime;
            TimeSpanBetweenClipsAsTicks = channelContainer.TimeSpanBetweenClipsAsTicks;
            Broadcasters = broadcasters;
        }
    }
}
