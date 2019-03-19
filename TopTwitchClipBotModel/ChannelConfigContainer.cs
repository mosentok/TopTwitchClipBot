using System.Collections.Generic;
using System.Linq;

namespace TopTwitchClipBotModel
{
    public class ChannelConfigContainer
    {
        public decimal ChannelId { get; set; }
        public string Prefix { get; set; }
        public int? MinPostingHour { get; set; }
        public int? MaxPostingHour { get; set; }
        public int? NumberOfClipsAtATime { get; set; }
        public long? TimeSpanBetweenClipsAsTicks { get; set; }
        public int? GlobalMinViews { get; set; }
        public List<BroadcasterConfigContainer> Broadcasters { get; set; }
        public ChannelConfigContainer() { }
        public ChannelConfigContainer(ChannelConfig channelConfig)
        {
            ChannelId = channelConfig.ChannelId;
            Prefix = channelConfig.Prefix;
            MinPostingHour = channelConfig.MinPostingHour;
            MaxPostingHour = channelConfig.MaxPostingHour;
            NumberOfClipsAtATime = channelConfig.NumberOfClipsAtATime;
            TimeSpanBetweenClipsAsTicks = channelConfig.TimeSpanBetweenClipsAsTicks;
            GlobalMinViews = channelConfig.GlobalMinViews;
            Broadcasters = channelConfig.BroadcasterConfigs.Select(s => new BroadcasterConfigContainer(s)).ToList();
        }
        public ChannelConfigContainer FromPostingHours(int? minPostingHour, int? maxPostingHour)
        {
            return new ChannelConfigContainer
            {
                ChannelId = ChannelId,
                Prefix = Prefix,
                MinPostingHour = minPostingHour,
                MaxPostingHour = maxPostingHour,
                NumberOfClipsAtATime = NumberOfClipsAtATime,
                TimeSpanBetweenClipsAsTicks = TimeSpanBetweenClipsAsTicks,
                GlobalMinViews = GlobalMinViews
            };
        }
        public ChannelConfigContainer FromClipsAtATime(int? numberOfClipsAtATime)
        {
            return new ChannelConfigContainer
            {
                ChannelId = ChannelId,
                Prefix = Prefix,
                MinPostingHour = MinPostingHour,
                MaxPostingHour = MaxPostingHour,
                NumberOfClipsAtATime = numberOfClipsAtATime,
                TimeSpanBetweenClipsAsTicks = TimeSpanBetweenClipsAsTicks,
                GlobalMinViews = GlobalMinViews
            };
        }
        public ChannelConfigContainer FromTimeSpanBetweenClipsAsTicks(long? timeSpanBetweenClipsAsTicks)
        {
            return new ChannelConfigContainer
            {
                ChannelId = ChannelId,
                Prefix = Prefix,
                MinPostingHour = MinPostingHour,
                MaxPostingHour = MaxPostingHour,
                NumberOfClipsAtATime = NumberOfClipsAtATime,
                TimeSpanBetweenClipsAsTicks = timeSpanBetweenClipsAsTicks,
                GlobalMinViews = GlobalMinViews
            };
        }
        public ChannelConfigContainer FromGlobalMinViews(int? globalMinViews)
        {
            return new ChannelConfigContainer
            {
                ChannelId = ChannelId,
                Prefix = Prefix,
                MinPostingHour = MinPostingHour,
                MaxPostingHour = MaxPostingHour,
                NumberOfClipsAtATime = NumberOfClipsAtATime,
                TimeSpanBetweenClipsAsTicks = TimeSpanBetweenClipsAsTicks,
                GlobalMinViews = globalMinViews
            };
        }
    }
}
